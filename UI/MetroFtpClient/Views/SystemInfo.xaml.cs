using System.Windows.Controls;

namespace MetroFtpClient.Views
{
    /// <summary>
    /// Interaktionslogik für SystemInfo.xaml
    /// </summary>
    public partial class SystemInfo : UserControl
    {
        public SystemInfo()
        {
            InitializeComponent();
        }

        private void OnRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}