using Prism.Mvvm;

namespace MetroFtpClient.Model
{
    public class TabContent : BindableBase
    {
        public TabContent(string header, object headerIcon, object content)
        {
            Header = header;
            HeaderIcon = headerIcon;
            Content = content;            
        }

        private string header;

        public string Header
        {
            get { return header; }
            private set { this.SetProperty<string>(ref this.header, value); }
        }

        private object headerIcon;

        public object HeaderIcon
        {
            get { return headerIcon; }
            set { this.SetProperty<object>(ref this.headerIcon, value); }
        }

        private object content;

        public object Content
        {
            get { return content; }
            private set { this.SetProperty<object>(ref this.content, value); }
        }
    }
}