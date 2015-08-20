using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Shared_Methods;
using System.Threading;
using System.Diagnostics;

namespace Yet_Another_Game_Patcher {

    public partial class MainWindow : Window {

        Dictionary<String, String> Options = new Dictionary<String, String>();

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Initialized(Object sender, EventArgs e) {
            var settings = AppDomain.CurrentDomain.BaseDirectory + "\\Patcher.xml";
            if (File.Exists(settings)) {
                try {
                    foreach (var el in XElement.Load(File.OpenRead(settings)).Elements()) {
                        Options.Add(el.Attribute("key").Value, el.Attribute("value").Value);
                    }
                    this.Title = Options["GameName"];
                    Browser.Navigate(Options["NewsPage"]);
                } catch { ReportError("An error has occured while attempting to apply the settings."); return; }
            } else {
                ReportError("Unable to locate 'Patcher.xml'!"); 
                return;
            }

            try {
                var data = new WebClient().DownloadData(Options["RemoteDirectory"] + "Checksums.xml");
                var remotefiles = XElement.Load(new MemoryStream(data));
                var worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(CompareFiles);
                worker.RunWorkerCompleted += (s, a) => { PlayButton.Dispatcher.BeginInvoke((Action)(() => { ContentLabel.Content = "Ready."; PlayButton.IsEnabled = true; })); };
                worker.RunWorkerAsync(remotefiles);
            } catch { ReportError("An error has occured while processing the remote data."); return; }
        }

        private void CompareFiles(Object sender, DoWorkEventArgs e) {
            var files   = e.Argument as XElement;
            var total   = files.Elements().Count();
            var current = 0;
            this.ThreadSafe(() => { TotalProgress.Maximum = total; });
            try {
                foreach (var file in files.Elements()) {
                    var dir = AppDomain.CurrentDomain.BaseDirectory + "\\";
                    var name = file.Attribute("name").Value;
                    var rhash = file.Attribute("hash").Value;
                    var download = false;
                    this.ThreadSafe(() => { ContentLabel.Content = String.Format("Checking {0}...", name); });
                    if (!File.Exists(dir + name)) {
                        download = true;
                    } else {
                        using (var f = File.OpenRead(dir + name)) {
                            var lhash = f.ToSha256();
                            if (!String.Equals(lhash, rhash)) download = true;
                        }
                    }

                    if (download) {
                        this.ThreadSafe(() => { ContentLabel.Content = String.Format("Downloading {0}...", name); });
                        if (!Directory.Exists(System.IO.Path.GetDirectoryName(dir + name))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dir + name));
                        if (File.Exists(dir + name)) File.Delete(dir + name);
                        using (var client = new WebClient()) {
                            try {
                                client.DownloadFile(Options["RemoteDirectory"] + name, dir + name);
                            } catch { ReportError(String.Format("An error has occured while attempting to download '{0}'", name)); return; }
                        }
                    }

                    current++;
                    this.ThreadSafe(() => { TotalProgress.Value = current; });
                }
            } catch { ReportError("A fatal error has occured while attempting to process the updates."); return; }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            var dir = AppDomain.CurrentDomain.BaseDirectory + "\\";
            try {
                Process.Start(dir + Options["GameClient"]);
            } catch { ReportError(String.Format("Unable to locate '{0}'!", Options["GameClient"])); return; }
        }

        private void ReportError(String msg) {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Close();
        }

    }
}
