using MetroFtpClient.Core.Base;
using MetroFtpClient.Infrastructure;
using MetroFtpClient.Infrastructure.Constants;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Windows.Input;

namespace MetroFtpClient.ViewModels
{
    public class LeftTitlebarWindowCommandsViewModel : ViewModelBase
    {
        public LeftTitlebarWindowCommandsViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            this.ShowApplicationHelpCommand = new DelegateCommand(this.ShowApplicationHelp);
            this.ShowApplicationInfoCommand = new DelegateCommand(this.ShowApplicationInfo);

            var applicationCommands = this.Container.Resolve<IApplicationCommands>();

            if (applicationCommands != null)
            {
                //applicationCommands.ShowApplicationHelpCommand.RegisterCommand(this.ShowApplicationHelpCommand);
                applicationCommands.ShowApplicationInfoCommand.RegisterCommand(this.ShowApplicationInfoCommand);
            }
        }

        #region Commands

        /// <summary>
        /// Show help command
        /// </summary>
        public ICommand ShowApplicationHelpCommand { get; private set; }

        /// <summary>
        /// Show info command
        /// </summary>
        public ICommand ShowApplicationInfoCommand { get; private set; }

        private void ShowApplicationHelp()
        {
            //this.UnityContainer.Resolve<IMessageDisplayService>(ServiceNames.MetroMessageDisplayService).ShowMessage("Not implemented", "Not yet implemented");
        }

        /// <summary>
        /// Show application information
        /// </summary>
        private void ShowApplicationInfo()
        {
            this.RegionManager.RequestNavigate(RegionNames.DialogPopupRegion, ViewNames.SystemInformationView);
        }

        #endregion Commands
    }
}