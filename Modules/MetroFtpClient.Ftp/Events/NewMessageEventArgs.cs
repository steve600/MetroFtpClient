using System;

namespace MetroFtpClient.Ftp.Events
{
    public class NewMessageEventArgs : EventArgs
    {
        public NewMessageEventArgs(string newMessage)
        {
            this.NewMessage = newMessage;
        }

        public string NewMessage { get; set; }
    }
}