using MahApps.Metro.Controls;
using MetroFtpClient.Infrastructure.Constants;
using MetroFtpClient.Infrastructure.Interfaces;

namespace MetroFtpClient.Views
{
    /// <summary>
    /// Interaktionslogik für ApplicationSettings.xaml
    /// </summary>
    public partial class ApplicationSettings : Flyout, IFlyoutView
    {
        public ApplicationSettings()
        {
            InitializeComponent();
        }

        public string FlyoutName
        {
            get
            {
                return FlyoutNames.ApplicationSettingsFlyout;
            }
        }
    }
}