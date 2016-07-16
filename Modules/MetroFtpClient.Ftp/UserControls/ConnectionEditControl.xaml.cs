using MetroFtpClient.Ftp.Contracts.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace MetroFtpClient.Ftp.UserControls
{
    /// <summary>
    /// Interaktionslogik für ConnectionEditControl.xaml
    /// </summary>
    public partial class ConnectionEditControl : UserControl
    {
        public ConnectionEditControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The connection
        /// </summary>
        public IConnectionSettings Connection
        {
            get { return (IConnectionSettings)GetValue(ConnectionProperty); }
            set { SetValue(ConnectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Connection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionProperty =
            DependencyProperty.Register("Connection", typeof(IConnectionSettings), typeof(ConnectionEditControl), new PropertyMetadata(null));
    }
}