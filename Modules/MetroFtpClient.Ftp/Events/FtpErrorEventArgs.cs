using System;

namespace MetroFtpClient.Ftp.Events
{
    public class FtpErrorEventArgs : EventArgs
    {
        public Exception ErrorException { get; set; }
    }
}