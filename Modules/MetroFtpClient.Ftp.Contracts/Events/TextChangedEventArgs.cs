using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroFtpClient.Ftp.Contracts.Events
{
    public class TextChangedEventArgs : EventArgs
    {
        public TextChangedEventArgs(string oldValue, string newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
