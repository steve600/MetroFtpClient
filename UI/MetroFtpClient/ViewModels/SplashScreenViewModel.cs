using MetroFtpClient.Core.Base;
using MetroFtpClient.Events;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using System;

namespace MetroFtpClient.ViewModels
{
    public class SplashScreenViewModel : ViewModelBase
    {
        #region CTOR

        public SplashScreenViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            string appVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            this.ApplicationDisplayString = String.Format("v{0}", appVersion);

            this.EventAggregator.GetEvent<SplashScreenStatusMessageUpdateEvent>().Subscribe(UpdateMessage);
        }

        #endregion CTOR

        #region Properties

        private string applicationDisplayString;

        /// <summary>
        /// Application display string
        /// </summary>
        public string ApplicationDisplayString
        {
            get { return applicationDisplayString; }
            set { this.SetProperty<string>(ref this.applicationDisplayString, value); }
        }

        private string splashScreenStatusMessage = string.Empty;

        /// <summary>
        /// Status message
        /// </summary>
        public string SplashScreenStatusMessage
        {
            get { return splashScreenStatusMessage; }
            set { this.SetProperty<string>(ref this.splashScreenStatusMessage, value); }
        }

        #endregion Properties

        #region Private Methods

        private void UpdateMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            this.SplashScreenStatusMessage += string.Concat(Environment.NewLine, message, "...");
        }

        #endregion Private Methods
    }
}