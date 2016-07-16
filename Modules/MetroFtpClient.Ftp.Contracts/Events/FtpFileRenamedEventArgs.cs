using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroFtpClient.Ftp.Contracts.Events
{
    public class FtpFileRenamedEventArgs : EventArgs
    {
        public FtpFileRenamedEventArgs(string oldValue, string newValue, Uri oldUrl, Uri newUrl)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.OldUrl = oldUrl;
            this.NewUrl = newUrl;
        }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public Uri OldUrl { get; set; }
        public Uri NewUrl { get; set; }
    }
}
