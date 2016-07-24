using MetroFtpClient.Ftp.ViewModels;
using Microsoft.WindowsAPICodePack.Shell;
using Prism.Commands;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace MetroFtpClient.Ftp.Views
{
    /// <summary>
    /// Interaktionslogik für FtpConnection.xaml
    /// </summary>
    public partial class FtpClient : UserControl
    {
        public FtpClient()
        {
            InitializeComponent();

            this.Loaded += FtpClient_Loaded;
            this.ftpFileSystemControl.FtpFileRenamedEvent += FtpFileSystemControl_FtpFileRenamedEvent;

            this.localFileExplorer.ExplorerBrowserControl.NavigationComplete += ExplorerBrowserControl_NavigationComplete;
        }

        private void ExplorerBrowserControl_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            var vm = this.DataContext as FtpClientViewModel;

            if (vm != null)
            {
                vm.SelectedDownloadDirectory = e.NewLocation.ParsingName;
            }
        }

        private void FtpFileSystemControl_FtpFileRenamedEvent(object sender, Contracts.Events.FtpFileRenamedEventArgs e)
        {
            var vm = this.DataContext as FtpClientViewModel;

            if (vm != null)
            {
                
            }
        }

        private void FtpClient_Loaded(object sender, RoutedEventArgs e)
        {
            this.localFileExplorer.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Downloads);
        }

        private void ListView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var vm = this.DataContext as FtpClientViewModel;

            if (vm != null)
            {
                ((DelegateCommand<IList>)vm.OpenFolderInExplorerCommand).RaiseCanExecuteChanged();
            }
        }
    }
}