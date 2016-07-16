using MetroFtpClient.Core.Base;
using MetroFtpClient.Ftp.Contracts.Interfaces;
using MetroFtpClient.Infrastructure.Constants;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Windows.Input;
using System;
using Prism.Interactivity.InteractionRequest;
using MetroFtpClient.Infrastructure.Notifications;
using MetroFtpClient.Infrastructure.Interfaces;
using MetroFtpClient.Ftp.FtpClient;
using System.Windows.Controls;
using System.Windows;

namespace MetroFtpClient.Ftp.ViewModels
{
    public class AddConnectionViewModel : ViewModelPopupBase, IInteractionRequestAware
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
        public AddConnectionViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            // Initialize commands
            this.InitializeCommands();

            this.connectionManager = this.Container.Resolve<IConnectionManager>(GlobalConstants.ConnectionManager);

            this.PropertyChanged += AddConnectionViewModel_PropertyChanged;

        }

        private void AddConnectionViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(this.Notification)))
            {
                if (this.Notification != null && this.Notification is AddOrUpdateConnectionNotification)
                {
                    switch (((AddOrUpdateConnectionNotification)this.Notification).DataOperation)
                    {
                        case Infrastructure.Enums.DataOperation.Insert:
                            this.NewConnection = new FtpConnectionSettings { Port = 21, Timeout = 10000 };
                            this.ConfirmButtonText = this.Container.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("AddConnectionButtonText");
                            this.ConfirmButtonImage = Application.Current.TryFindResource("appbar_add") as Canvas;
                            break;
                        case Infrastructure.Enums.DataOperation.Update:
                            this.NewConnection = ((AddOrUpdateConnectionNotification)this.Notification).ConnetionSettings;
                            this.ConfirmButtonText = this.Container.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("UpdateConnectionButtonText");
                            this.ConfirmButtonImage = Application.Current.TryFindResource("appbar_save") as Canvas;
                            break;
                        default:
                            this.ConfirmButtonText = this.Container.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("AddConnectionButtonText");
                            this.ConfirmButtonImage = Application.Current.TryFindResource("appbar_add") as Canvas;
                            break;
                    }                   
                }
            }
        }

        #region Commands

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            this.AddOrUpdatedConnectionCommand = new DelegateCommand(this.AddOrUpdateConnection, this.CanAddOrUpdateConnection);
        }

        /// <summary>
        /// Add connection command
        /// </summary>
        public ICommand AddOrUpdatedConnectionCommand { get; private set; }

        /// <summary>
        /// Add connection can execute
        /// </summary>
        /// <returns></returns>
        private bool CanAddOrUpdateConnection()
        {
            return true;
        }

        /// <summary>
        /// Add connection
        /// </summary>
        private void AddOrUpdateConnection()
        {
            // Set confirmed
            ((Confirmation)this.Notification).Confirmed = true;

            // Set connection settings
            ((AddOrUpdateConnectionNotification)this.Notification).ConnetionSettings = this.NewConnection;

            // Close popup
            this.ClosePopupCommand.Execute(null);
        }

        #endregion Commands

        #region Properties

        private IConnectionSettings newConnection;

        /// <summary>
        /// The new connection
        /// </summary>
        public IConnectionSettings NewConnection
        {
            get { return newConnection; }
            set { this.SetProperty<IConnectionSettings>(ref this.newConnection, value); }
        }

        private string confirmButtonText;

        /// <summary>
        /// The text for the confirm button
        /// </summary>
        public string ConfirmButtonText
        {
            get { return confirmButtonText; }
            set { this.SetProperty<string>(ref this.confirmButtonText, value); }
        }

        private Canvas confirmButtonImage;

        /// <summary>
        /// The image for the confirm button
        /// </summary>
        public Canvas ConfirmButtonImage
        {
            get { return confirmButtonImage; }
            set { this.SetProperty<Canvas>(ref this.confirmButtonImage, value); }
        }

        #endregion Properties
    }
}