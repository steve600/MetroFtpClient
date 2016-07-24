using MetroFtpClient.Ftp.Contracts.Interfaces;
using Prism.Commands;

namespace MetroFtpClient.Infrastructure
{
    /// <summary>
    /// Class with global application commands
    /// </summary>
    public static class ApplicationCommands
    {
        /// <summary>
        /// Show flyout command
        /// </summary>
        public static CompositeCommand ShowFlyoutCommand = new CompositeCommand();

        /// <summary>
        /// Show project on GitHub
        /// </summary>
        public static DelegateCommand ShowOnGitHubCommand = new DelegateCommand(ShowOnGitHub);

        /// <summary>
        /// Show on GitHub
        /// </summary>
        private static void ShowOnGitHub()
        {
            System.Diagnostics.Process.Start("https://github.com/steve600/MetroFtpClient");
        }

        /// <summary>
        /// Show application info
        /// </summary>
        public static CompositeCommand ShowApplicationInfoCommand = new CompositeCommand();

        /// <summary>
        /// Add connection command
        /// </summary>
        public static DelegateCommand AddConnectionCommand = null;

        /// <summary>
        /// Update connection command
        /// </summary>
        public static DelegateCommand<IConnectionSettings> UpdateConnectionCommand = null;

        /// <summary>
        /// Open connection command
        /// </summary>
        public static DelegateCommand<object> OpenConnectionCommand = null;

        /// <summary>
        /// Open connection command
        /// </summary>
        public static DelegateCommand<IConnectionSettings> DeleteConnectionCommand = null;
    }

    public interface IApplicationCommands
    {
        CompositeCommand ShowFlyoutCommand { get; }
        DelegateCommand ShowOnGitHubCommand { get; }
        CompositeCommand ShowApplicationInfoCommand { get; }
        DelegateCommand AddConnectionCommand { get; set; }
        DelegateCommand<IConnectionSettings> UpdateConnectionCommand { get; set; }
        DelegateCommand<object> OpenConnectionCommand { get; set; }
        DelegateCommand<IConnectionSettings> DeleteConnectionCommand { get; set; }
    }

    public class ApplicationCommandsProxy : IApplicationCommands
    {
        /// <summary>
        /// Show flyout command
        /// </summary>
        public CompositeCommand ShowFlyoutCommand
        {
            get { return ApplicationCommands.ShowFlyoutCommand; }
        }

        /// <summary>
        /// Show project on GitHub
        /// </summary>
        public DelegateCommand ShowOnGitHubCommand
        {
            get { return ApplicationCommands.ShowOnGitHubCommand; }
        }

        /// <summary>
        /// Show application info command
        /// </summary>
        public CompositeCommand ShowApplicationInfoCommand
        {
            get { return ApplicationCommands.ShowApplicationInfoCommand; }
        }

        /// <summary>
        /// Add connection command
        /// </summary>
        public DelegateCommand AddConnectionCommand
        {
            get { return ApplicationCommands.AddConnectionCommand; }
            set { ApplicationCommands.AddConnectionCommand = value; }
        }

        /// <summary>
        /// Update connection command
        /// </summary>
        public DelegateCommand<IConnectionSettings> UpdateConnectionCommand
        {
            get { return ApplicationCommands.UpdateConnectionCommand; }
            set { ApplicationCommands.UpdateConnectionCommand = value; }
        }

        /// <summary>
        /// Open connection command
        /// </summary>
        public DelegateCommand<object> OpenConnectionCommand
        {
            get { return ApplicationCommands.OpenConnectionCommand; }
            set { ApplicationCommands.OpenConnectionCommand = value; }
        }

        /// <summary>
        /// Delete connection command
        /// </summary>
        public DelegateCommand<IConnectionSettings> DeleteConnectionCommand
        {
            get { return ApplicationCommands.DeleteConnectionCommand; }
            set { ApplicationCommands.DeleteConnectionCommand = value; }
        }
    }
}