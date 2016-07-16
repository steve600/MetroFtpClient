using System;
using System.IO;

namespace MetroFtpClient.Ftp.Events
{
    public class FileDownloadCompletedEventArgs : EventArgs
    {
        public FileDownloadCompletedEventArgs(Uri serverPath, FileInfo localFile)
        {
            this.ServerPath = serverPath;
            this.LocalFile = localFile;
        }

        public Uri ServerPath { get; set; }
        public FileInfo LocalFile { get; set; }
    }
}