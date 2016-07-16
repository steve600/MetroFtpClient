using MetroFtpClient.Core.Base;
using MetroFtpClient.Ftp.Contracts.Interfaces;
using MetroFtpClient.Infrastructure.Constants;
using MetroFtpClient.Infrastructure.Interfaces;
using MetroFtpClient.Infrastructure.Notifications;
using MetroFtpClient.Infrastructure.Services;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows;

namespace MetroFtpClient.ViewModels
{
    public class OverviewPageViewModel : ViewModelBase
    {
        #region Members and Constants

        private IConnectionManager connectionManager = null;

        #endregion Members and Constants

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggrgator">The event aggregator.</param>
        public OverviewPageViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            this.Title = this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("OverviewPageTitle");
            this.Icon = Application.Current.FindResource("appbar_home_empty");

            this.connectionManager = this.Container.Resolve<IConnectionManager>(GlobalConstants.ConnectionManager);
        }

        #region Properties

        /// <summary>
        /// List with connections
        /// </summary>
        public ObservableCollection<IConnectionSettings> Connections
        {
            get
            {
                return this.connectionManager.Connections;
            }
        }

        #endregion Properties
    }
}