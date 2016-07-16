using MetroFtpClient.Core.Base;
using MetroFtpClient.Infrastructure.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System;
using System.Windows;

namespace MetroFtpClient.Ftp
{
    public class FtpModule : PrismModuleBase
    {
        public FtpModule(IUnityContainer unityContainer, RegionManager regionManager) :
            base(unityContainer, regionManager)
        {
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public override void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion(RegionNames.RightWindowCommandsRegion, typeof(Views.FtpRightTitlebarWindowCommands));
            this.RegionManager.RegisterViewWithRegion(RegionNames.FlyoutRegion, typeof(Views.FtpConnectionsFlyout));

            // Register views for navigation
            //Prism.Unity.UnityExtensions.RegisterTypeForNavigation<Views.AddOrUpdateConnection>(this.UnityContainer, ViewNames.AddConnectionView);

        }

        /// <summary>
        /// Add Resource-Dictionaries
        /// </summary>
        public override void AddResourceDictionaries()
        {
            var rd = new ResourceDictionary();

            rd.Source = new Uri("/MetroFtpClient.Ftp;component/Styling/LookAndFeel.xaml", UriKind.RelativeOrAbsolute);

            Application.Current.Resources.MergedDictionaries.Add(rd);
        }
    }
}