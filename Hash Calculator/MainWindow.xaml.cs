using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Forms;                             // Ewww, System.Windows.Forms Microsoft, please!
using System.ComponentModel;
using System.Xml.Linq;
using Shared_Methods;

namespace Hash_Calculator {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Select_Click(Object sender, RoutedEventArgs e) {
            using (var d = new FolderBrowserDialog()) {
                var r = d.ShowDialog();
                if (r == System.Windows.Forms.DialogResult.OK) FolderLocation.Text = d.SelectedPath;
            }   
        }

        private void Button_Go_Click(Object sender, RoutedEventArgs e) {
            if (FolderLocation.Text.Length > 0) {
                ButtonGo.Content = "Working...";
                ButtonGo.IsEnabled = false;

                using (var worker = new BackgroundWorker()) {
                    worker.DoWork += new DoWorkEventHandler(GenerateHashes);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GenerateComplete);
                    worker.RunWorkerAsync(FolderLocation.Text);
                }
            } else {
                System.Windows.Forms.MessageBox.Show("Unable to proceed, please select a folder!", "Oopsie!", MessageBoxButtons.OK);
            }
        }

        private void GenerateComplete(Object sender, RunWorkerCompletedEventArgs e) {
            ButtonGo.Dispatcher.BeginInvoke((Action)(() => { ButtonGo.Content = "Go!"; ButtonGo.IsEnabled = true; }));
            System.Windows.Forms.MessageBox.Show("Done!");
        }

        private void GenerateHashes(Object sender, DoWorkEventArgs e) {
            var folder = ((String)e.Argument) + "\\";
            var file    = AppDomain.CurrentDomain.BaseDirectory + "\\output.xml";
            var info 	= new DirectoryInfo(folder);
	        
            if (File.Exists(file)) File.Delete(file);

            new XDocument(
                 new XElement("files",
                     (from f in info.GetFiles("*.*", SearchOption.AllDirectories)
                      let filename = f.FullName.Replace(folder, "")
                      let hash = File.OpenRead(f.FullName).ToSha256()
                      select new XElement("file", new XAttribute("name", filename), new XAttribute("hash", hash)))
                )
            ).Save(file);
        }

    }
}
