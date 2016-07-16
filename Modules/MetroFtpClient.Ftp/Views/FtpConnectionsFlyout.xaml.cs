using MahApps.Metro.Controls;
using MetroFtpClient.Infrastructure.Constants;
using MetroFtpClient.Infrastructure.Interfaces;

namespace MetroFtpClient.Ftp.Views
{
    /// <summary>
    /// Interaktionslogik für FtpConnectionsFlyout.xaml
    /// </summary>
    public partial class FtpConnectionsFlyout : Flyout, IFlyoutView
    {
        public FtpConnectionsFlyout()
        {
            InitializeComponent();
        }

        public string FlyoutName
        {
            get
            {
                return FlyoutNames.FtpConnectionsFlyout;
            }
        }
    }
}