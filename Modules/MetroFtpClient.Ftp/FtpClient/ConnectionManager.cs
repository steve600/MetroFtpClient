using MahApps.Metro.Controls.Dialogs;
using MetroFtpClient.Ftp.Contracts.Events;
using MetroFtpClient.Ftp.Contracts.Interfaces;
using MetroFtpClient.Ftp.ViewModels;
using MetroFtpClient.Infrastructure.Constants;
using MetroFtpClient.Infrastructure.Enums;
using MetroFtpClient.Infrastructure.Events;
using MetroFtpClient.Infrastructure.Interfaces;
using MetroFtpClient.Infrastructure.Notifications;
using MetroFtpClient.Infrastructure.Services;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace MetroFtpClient.Ftp.FtpClient
{
    public class ConnectionManager : IConnectionManager
    {
        #region Members and Constants

        private IConnectionFactory connectionFactory = null;
        private IEventAggregator eventAggregator = null;
        private IUnityContainer unityContainer = null;

        #endregion Members and Constants

        /// <summary>
        /// CTOR
        /// </summary>
        public ConnectionManager(IUnityContainer unityContainer, IEventAggregator eventAggregator)
        {
            this.unityContainer = unityContainer;
            this.eventAggregator = eventAggregator;

            this.connectionFactory = this.unityContainer.Resolve<ConnectionFactory>();

            // TODO: Is this the right place to do this?
            unityContainer.RegisterInstance<InteractionRequest<AddOrUpdateConnectionNotification>>(InteractionRequests.ShowAddOrUpdateConnectionPopupRequest, new InteractionRequest<AddOrUpdateConnectionNotification>());

            // Register commands
            Infrastructure.ApplicationCommands.AddConnectionCommand = new DelegateCommand(AddConnection);
            Infrastructure.ApplicationCommands.UpdateConnectionCommand = new DelegateCommand<IConnectionSettings>(RaiseUpdateConnectionCommand);
            Infrastructure.ApplicationCommands.OpenConnectionCommand = DelegateCommand<object>.FromAsyncHandler(OpenConnection);
            Infrastructure.ApplicationCommands.DeleteConnectionCommand = DelegateCommand<IConnectionSettings>.FromAsyncHandler(RaiseDeleteConnectionCommand, CanRaiseDeleteConnectionCommand);

            // Read config file
            this.ReadConfigFile();
        }

        #region Commands

        /// <summary>
        /// Open connection
        /// </summary>
        /// <param name="connectionSettings">The connection settings.</param>
        private async Task OpenConnection(object connectionSettings)
        {
            if (connectionSettings != null && connectionSettings is IConnectionSettings)
            {
                IRegionManager regionManager = unityContainer.Resolve<IRegionManager>();
                
                var view = unityContainer.Resolve<Views.FtpClient>();
                if (view.DataContext != null && view.DataContext is FtpClientViewModel)
                {
                    // TODO
                    ((FtpClientViewModel)view.DataContext).Title = ((IConnectionSettings)connectionSettings).Name;

                    regionManager.AddToRegion(RegionNames.MainRegion, view);

                    await ((FtpClientViewModel)view.DataContext).InitializeConnection(connectionSettings as IConnectionSettings);
                }                
            }
        }

        /// <summary>
        /// Delete connection
        /// </summary>
        /// <param name="connectionSettings">The connection settings.</param>
        /// <returns></returns>
        private async Task RaiseDeleteConnectionCommand(IConnectionSettings connectionSettings)
        {
            if (connectionSettings != null)
            {
                IRegionManager regionManager = unityContainer.Resolve<IRegionManager>();

                var messageDisplayService = unityContainer.Resolve<IMetroMessageDisplayService>(ServiceNames.MetroMessageDisplayService);

                MetroDialogSettings dialogSettings = new MetroDialogSettings { AffirmativeButtonText = "OK", NegativeButtonText = "Cancel" };

                var result = await messageDisplayService.ShowMessageAsnyc($"Verbindung {connectionSettings.Name} löschen", "Soll die Verbindung wirklich gelöscht werden?", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);

                if (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                {
                    this.DeleteConnection(connectionSettings);
                }
            }
        }

        private bool CanRaiseDeleteConnectionCommand(IConnectionSettings connectionSettings)
        {
            return connectionSettings != null;
        }

        /// <summary>
        /// Delete connection
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private bool DeleteConnection(IConnectionSettings connection)
        {
            bool result = true;
            string statusBarMessage = string.Empty;

            // Delete connection
            this.Connections.Remove(connection);
            result = this.DeleteConnectionFromConfig(connection);

            if (result)
            {
                this.eventAggregator.GetEvent<ConnectionUpdatedEvent>().Publish(new ConnectionUpdatedEventArgs());
                statusBarMessage = this.unityContainer.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("MetroFtpClient.Ftp:Resources:ConnectionDeletedSuccessfullStatusBarMessage");
            }
            else
                statusBarMessage = this.unityContainer.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("MetroFtpClient.Ftp:Resources:ConnectionDeletedFailedStatusBarMessage");

            // Update status bar
            this.eventAggregator.GetEvent<UpdateStatusBarMessageEvent>().Publish(String.Format(statusBarMessage, connection?.Name));

            return result;
        }

        /// <summary>
        /// Add a new connection
        /// </summary>
        private void AddConnection()
        {
            AddOrUpdateConnectionNotification notification = new AddOrUpdateConnectionNotification(DataOperation.Insert);

            notification.Title = "Verbindung anlegen";

            var ir = this.unityContainer.Resolve<InteractionRequest<AddOrUpdateConnectionNotification>>(InteractionRequests.ShowAddOrUpdateConnectionPopupRequest);

            ir.Raise(notification,
                returned =>
                {
                    if (returned != null && returned.Confirmed)
                    {
                        if (returned.ConnetionSettings != null)
                        {
                            if (returned.DataOperation == DataOperation.Insert)
                            {
                                this.AddConnection(returned.ConnetionSettings);
                            }                            
                        }
                    }
                });
        }

        /// <summary>
        /// Update existing connection
        /// </summary>
        /// <param name="connectionSettings">The connection settings</param>
        private void RaiseUpdateConnectionCommand(IConnectionSettings connectionSettings)
        {
            // Crate a copy of the original settings -> the Popupdialog works with the copy
            // --> the old settings are needed to search the entry in the config file
            var newConnectionSettings = ((ConnectionSettingsBase)connectionSettings).Clone() as IConnectionSettings;

            AddOrUpdateConnectionNotification notification = new AddOrUpdateConnectionNotification(DataOperation.Update, newConnectionSettings);
            
            notification.Title = "Verbindung bearbeiten";

            var ir = this.unityContainer.Resolve<InteractionRequest<AddOrUpdateConnectionNotification>>(InteractionRequests.ShowAddOrUpdateConnectionPopupRequest);

            ir.Raise(notification,
                returned =>
                {
                    if (returned != null && returned.Confirmed)
                    {
                        if (returned.ConnetionSettings != null)
                        {
                            if (returned.DataOperation == DataOperation.Update)
                            {
                                this.UpdateConnection(returned.ConnetionSettings, connectionSettings);
                            }
                        }
                    }
                });
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Read config file
        /// </summary>
        private void ReadConfigFile()
        {
            if (!System.IO.File.Exists(FileAndFolderConstants.ConnectionSettingsConfigFile))
            {
                this.CreateSettingsFile();
            }

            try
            {
                XElement xmlDoc = XElement.Load(FileAndFolderConstants.ConnectionSettingsConfigFile);

                foreach (var c in xmlDoc.Descendants("Connection"))
                {
                    ConnectionTypes connectionType;
                    Enum.TryParse<ConnectionTypes>(c.Attribute("ConnectionType")?.Value ?? string.Empty, out connectionType);

                    // Create connection
                    IConnectionSettings connection = this.CreateConnection(connectionType);

                    if (connection != null)
                    {
                        connection.Name = c.Attribute("Name")?.Value ?? string.Empty;
                        connection.Host = c.Attribute("Host")?.Value ?? string.Empty;
                        connection.Port = Convert.ToInt32(c.Attribute("Port")?.Value ?? "0");
                        connection.UserName = c.Attribute("UserName")?.Value ?? string.Empty;
                        connection.Password = c.Attribute("Password")?.Value ?? string.Empty;
                        connection.Timeout = Convert.ToInt32(c.Attribute("Timeout")?.Value ?? "10000");
                    }

                    this.Connections.Add(connection);
                }
            }
            catch (Exception ex)
            {
                // Extended properties
                Dictionary<string, object> extProps = new Dictionary<string, object>();
                extProps.Add("File-Path", FileAndFolderConstants.ConnectionSettingsConfigFile);
                // FtpLog-Exception
                //this.unityContainer.Resolve<ILoggingService>(ServiceNames.LoggingService).Write(ex, LoggingCategories.ConnectionManagementModule, 1, 1000, System.Diagnostics.TraceEventType.Error, "ConnectionManager :: Failed to read config file", extProps);
                // TODO: Show-Exeption
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Create settings file
        /// </summary>
        private void CreateSettingsFile()
        {
            try
            {
                // Create new XML-File
                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                    new XComment("Connection-Settings"),
                    new XElement("Connections"));

                xdoc.Save(FileAndFolderConstants.ConnectionSettingsConfigFile);
            }
            catch (Exception ex)
            {
                // Extended properties
                Dictionary<string, object> extProps = new Dictionary<string, object>();
                extProps.Add("File-Path", FileAndFolderConstants.ConnectionSettingsConfigFile);
                // FtpLog-Exception
                //this.unityContainer.Resolve<ILoggingService>(ServiceNames.LoggingService).Write(ex, LoggingCategories.ConnectionManagementModule, 1, 1000, System.Diagnostics.TraceEventType.Error, "ConnectionManager :: Failed to create config file", extProps);
                // TODO: Show-Exeption
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Add connection to config
        /// </summary>
        /// <param name="connection"></param>
        private bool AddConnectionToConfig(IConnectionSettings connection)
        {
            bool result = true;

            try
            {
                XDocument xmlDoc = XDocument.Load(FileAndFolderConstants.ConnectionSettingsConfigFile);

                xmlDoc.Element("Connections").Add(connection.ToXml());

                xmlDoc.Save(FileAndFolderConstants.ConnectionSettingsConfigFile);
            }
            catch (Exception ex)
            {
                // Extended properties
                Dictionary<string, object> extProps = new Dictionary<string, object>();
                extProps.Add("Connection", connection);
                // FtpLog-Exception
                //this.unityContainer.Resolve<ILoggingService>(ServiceNames.LoggingService).Write(ex, LoggingCategories.ConnectionManagementModule, 1, 1000, System.Diagnostics.TraceEventType.Error, "ConnectionManager :: Failed to add a connection", extProps);
                // TODO: Show-Exeption
                System.Diagnostics.Debug.WriteLine(ex);

                result = false;
            }

            return result;
        }

        /// <summary>
        /// Delete connection from config
        /// </summary>
        /// <param name="connection"></param>
        private bool DeleteConnectionFromConfig(IConnectionSettings connection)
        {
            bool result = true;

            try
            {
                XDocument xmlDoc = XDocument.Load(FileAndFolderConstants.ConnectionSettingsConfigFile);

                var configEntry = (from c in xmlDoc.Element("Connections").Elements()
                                   where c.Attribute(nameof(connection.Name)).Value == connection.Name &&
                                         c.Attribute(nameof(connection.Host)).Value == connection.Host &&
                                         Convert.ToInt32(c.Attribute(nameof(connection.Port)).Value) == connection.Port
                                   select c).FirstOrDefault();

                if (configEntry != null)
                {
                    configEntry.Remove();

                    xmlDoc.Save(FileAndFolderConstants.ConnectionSettingsConfigFile);
                }
            }
            catch (Exception ex)
            {
                // Extended properties
                Dictionary<string, object> extProps = new Dictionary<string, object>();
                extProps.Add("Connection", connection);
                // FtpLog-Exception
                //this.unityContainer.Resolve<ILoggingService>(ServiceNames.LoggingService).Write(ex, LoggingCategories.ConnectionManagementModule, 1, 1000, System.Diagnostics.TraceEventType.Error, "ConnectionManager :: Failed to delete a connection", extProps);
                // TODO: Show-Exeption
                System.Diagnostics.Debug.WriteLine(ex);

                result = false;
            }

            return result;
        }

        /// <summary>
        /// Update connection settings
        /// </summary>
        /// <param name="newConnectionSettings">The updated connection settings</param>
        /// <param name="oldConnectionSettings">The old connection settings</param>
        /// <returns></returns>
        private bool UpdateConnection(IConnectionSettings newConnectionSettings, IConnectionSettings oldConnectionSettings)
        {
            bool result = true;

            try
            {
                XDocument xmlDoc = XDocument.Load(FileAndFolderConstants.ConnectionSettingsConfigFile);

                // Get the original config entry (based on the old connection settings)
                var configEntry = (from c in xmlDoc.Element("Connections").Elements()
                                   where c.Attribute(nameof(oldConnectionSettings.Name)).Value == oldConnectionSettings.Name &&
                                         c.Attribute(nameof(oldConnectionSettings.Host)).Value == oldConnectionSettings.Host &&
                                         Convert.ToInt32(c.Attribute(nameof(oldConnectionSettings.Port)).Value) == oldConnectionSettings.Port
                                   select c).FirstOrDefault();

                if (configEntry != null)
                {
                    // Update the existing config entry with the new values
                    configEntry.Attribute(nameof(newConnectionSettings.Name)).Value = newConnectionSettings.Name;
                    configEntry.Attribute(nameof(newConnectionSettings.Host)).Value = newConnectionSettings.Host;
                    configEntry.Attribute(nameof(newConnectionSettings.ConnectionType)).Value = newConnectionSettings.ConnectionType.ToString();
                    configEntry.Attribute(nameof(newConnectionSettings.Port)).Value = newConnectionSettings.Port.ToString();
                    configEntry.Attribute(nameof(newConnectionSettings.Timeout)).Value = newConnectionSettings.Timeout.ToString();
                    configEntry.Attribute(nameof(newConnectionSettings.UserName)).Value = newConnectionSettings.UserName;
                    configEntry.Attribute(nameof(newConnectionSettings.Password)).Value = newConnectionSettings.Password;

                    xmlDoc.Save(FileAndFolderConstants.ConnectionSettingsConfigFile);

                    oldConnectionSettings.LoadFromXml(configEntry);
                }
            }
            catch (Exception ex)
            {
                // Extended properties
                Dictionary<string, object> extProps = new Dictionary<string, object>();
                extProps.Add("Connection", newConnectionSettings);
                // FtpLog-Exception
                //this.unityContainer.Resolve<ILoggingService>(ServiceNames.LoggingService).Write(ex, LoggingCategories.ConnectionManagementModule, 1, 1000, System.Diagnostics.TraceEventType.Error, "ConnectionManager :: Failed to delete a connection", extProps);
                // TODO: Show-Exeption
                System.Diagnostics.Debug.WriteLine(ex);

                result = false;
            }

            return result;
        }

        #endregion Methods

        #region Properties

        private IConnectionSettings activeConnection;

        /// <summary>
        /// The active connectionSettings
        /// </summary>
        public IConnectionSettings ActiveConnection
        {
            get { return activeConnection; }
            set { activeConnection = value; }
        }

        private ObservableCollection<IConnectionSettings> connections = new ObservableCollection<IConnectionSettings>();

        /// <summary>
        /// List with connections
        /// </summary>
        public ObservableCollection<IConnectionSettings> Connections
        {
            get { return connections; }
            set { connections = value; }
        }

        /// <summary>
        /// Create connection
        /// </summary>
        /// <param name="connectionKind"></param>
        /// <param name="connectionType"></param>
        /// <returns></returns>
        public IConnectionSettings CreateConnection(ConnectionTypes connectionType)
        {
            return this.connectionFactory.GetConnection(connectionType);
        }

        /// <summary>
        /// Add connection
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool AddConnection(IConnectionSettings connection)
        {
            bool result = true;
            string statusBarMessage = string.Empty;

            // Add connection
            this.Connections.Add(connection);
            result = this.AddConnectionToConfig(connection);

            if (result)
            {
                this.eventAggregator.GetEvent<ConnectionUpdatedEvent>().Publish(new ConnectionUpdatedEventArgs());
                statusBarMessage = this.unityContainer.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("MetroFtpClient.Ftp:Resources:ConnectionAddedSuccessfullStatusBarMessage");
            }
            else
                statusBarMessage = this.unityContainer.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("MetroFtpClient.Ftp:Resources:ConnectionAddedFailedStatusBarMessage");

            // Update status bar
            this.eventAggregator.GetEvent<UpdateStatusBarMessageEvent>().Publish(String.Format(statusBarMessage, connection?.Name));

            return result;
        }

        #endregion Properties
    }
}