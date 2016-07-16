using MetroFtpClient.Core.Base;
using MetroFtpClient.Ftp.Contracts.Interfaces;
using MetroFtpClient.Infrastructure.Constants;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using Prism.Interactivity.InteractionRequest;
using MetroFtpClient.Infrastructure.Notifications;

namespace MetroFtpClient.Ftp.ViewModels
{
    public class FtpConnectionsFlyoutViewModel : ViewModelBase
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
        public FtpConnectionsFlyoutViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            // Connection manager
            this.connectionManager = this.Container.Resolve<IConnectionManager>(GlobalConstants.ConnectionManager);            
        }

        #region Commands

        /// <summary>
        /// Check can execute of a command changed
        /// </summary>
        private void CheckRaiseCanExecuteChanged()
        {
            Infrastructure.ApplicationCommands.DeleteConnectionCommand.RaiseCanExecuteChanged();
            //((DelegateCommand)this.DeleteConnectionCommand).RaiseCanExecuteChanged();
        }

        #endregion Commands

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

        private IConnectionSettings selectedConnection;

        /// <summary>
        /// The selected connection
        /// </summary>
        public IConnectionSettings SelectedConnection
        {
            get { return selectedConnection; }
            set
            {
                if (this.SetProperty<IConnectionSettings>(ref this.selectedConnection, value))
                {
                    this.CheckRaiseCanExecuteChanged();
                }
            }
        }

        #endregion Properties
    }
}