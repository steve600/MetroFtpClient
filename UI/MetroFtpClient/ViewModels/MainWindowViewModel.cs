using Dragablz;
using MetroFtpClient.Core.Base;
using MetroFtpClient.Core.Configuration;
using MetroFtpClient.Core.Interfaces;
using MetroFtpClient.Infrastructure.Constants;
using MetroFtpClient.Infrastructure.Events;
using MetroFtpClient.Infrastructure.Interfaces;
using MetroFtpClient.Infrastructure.Notifications;
using MetroFtpClient.Model;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;

namespace MetroFtpClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggrgator">The event aggregator.</param>
        public MainWindowViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            this.Title = "Metro FTP Client";

            // Register to events
            EventAggregator.GetEvent<UpdateStatusBarMessageEvent>().Subscribe(OnUpdateStatusBarMessageEventHandler);

            _interTabClient = new DefaultInterTabClient();

            this.ShowAddOrUpdateConnectionPopupRequest = this.Container.Resolve<InteractionRequest<AddOrUpdateConnectionNotification>>(InteractionRequests.ShowAddOrUpdateConnectionPopupRequest);

            // Load application config
            this.LoadApplicationConfigFile();
        }

        #region Event-Handler

        private void OnUpdateStatusBarMessageEventHandler(string statusBarMessage)
        {
            this.StatusBarMessage = statusBarMessage;
        }

        #endregion Event-Handler

        #region TabControl

        private readonly IInterTabClient _interTabClient;

        public IInterTabClient InterTabClient
        {
            get { return _interTabClient; }
        }

        /// <summary>
        /// Callback to handle tab closing.
        /// </summary>
        public ItemActionCallback ClosingTabItemHandler
        {
            get { return ClosingTabItemHandlerImpl; }
        }

        /// <summary>
        /// Callback to handle tab closing.
        /// </summary>
        private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            //in here you can dispose stuff or cancel the close

            //here's your view model:
            var viewModel = args.DragablzItem.DataContext as TabContent;

            if (viewModel != null)
            {
                string title = Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("OverviewPageTitle");

                if (viewModel.Header.Equals(title))
                {
                    args.Cancel();
                }
            }

            //Debug.Assert(viewModel != null);

            //here's how you can cancel stuff:
            //args.Cancel();
        }

        #endregion TabControl

        /// <summary>
        /// Load configuration file
        /// </summary>
        /// <returns></returns>
        private IConfigurationFile LoadApplicationConfigFile()
        {
            // Create config file
            IConfigurationFile configFile = new XmlConfigurationFile(FileAndFolderConstants.ApplicationConfigFile);

            if (!configFile.Load())
            {
                // Add section GeneralSettings
                configFile.Sections.Add("GeneralSettings");
                this.CheckGeneralSettings(configFile);
            }
            else
            {
                // Check general settings
                this.CheckGeneralSettings(configFile);
            }

            // Save config file
            configFile.Save();

            // Register global application file
            this.Container.RegisterInstance<IConfigurationFile>(FileAndFolderConstants.ApplicationConfigFile, configFile);

            return configFile;
        }

        /// <summary>
        /// Check general settings
        /// </summary>
        /// <param name="configFile"></param>
        private void CheckGeneralSettings(IConfigurationFile configFile)
        {
            // Check if section exists
            if (configFile.Sections["GeneralSettings"] == null)
                configFile.Sections.Add("GeneralSettings");

            // Check settings
            if (configFile.Sections["GeneralSettings"].Settings["Theme"] == null)
                configFile.Sections["GeneralSettings"].Settings.Add("Theme", "BaseLight", "BaseLight", typeof(System.String));
            if (configFile.Sections["GeneralSettings"].Settings["Language"] == null)
                configFile.Sections["GeneralSettings"].Settings.Add("Language", "de", "de", typeof(System.String));
            if (configFile.Sections["GeneralSettings"].Settings["AccentColor"] == null)
                configFile.Sections["GeneralSettings"].Settings.Add("AccentColor", "Cyan", "Cyan", typeof(System.String));
        }

        #region Properties

        private string statusBarMessage;

        /// <summary>
        /// Status-Bar message
        /// </summary>
        public string StatusBarMessage
        {
            get { return statusBarMessage; }
            private set { this.SetProperty<string>(ref this.statusBarMessage, value); }
        }

        private InteractionRequest<AddOrUpdateConnectionNotification> showAddOrUpdateConnectionPopupRequest = null;

        /// <summary>
        /// Interaction request to show the add or update connection popup
        /// </summary>
        public InteractionRequest<AddOrUpdateConnectionNotification> ShowAddOrUpdateConnectionPopupRequest
        {
            get
            {
                return this.showAddOrUpdateConnectionPopupRequest;
            }
            private set
            {
                this.SetProperty<InteractionRequest<AddOrUpdateConnectionNotification>>(ref this.showAddOrUpdateConnectionPopupRequest, value);
            }
        }

        #endregion Properties
    }
}