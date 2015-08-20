using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Threading;

namespace Shared_Methods {
    public static class Extensions {

        #region Cryptography
        /// <summary>
        /// Creates a Sha256 Hash of the supplied FileStream.
        /// </summary>
        /// <param name="file">The FileStream to generate a Hash from.</param>
        /// <returns>Returns a String containing a Sha256 hash.</returns>
        public static String ToSha256(this FileStream file) {
            var sha = new SHA256Managed();
            return BitConverter.ToString(sha.ComputeHash(file)).Replace("-", "");
        }
        #endregion

        #region WPF
        /// <summary>
        /// Invokes the supplied action onto the supplied Window.
        /// </summary>
        /// <param name="win">The destination window</param>
        /// <param name="act">The action to execute</param>
        public static void ThreadSafe(this Window win, Action act) {
            win.Dispatcher.BeginInvoke(act);
        }
        #endregion

    }
}
