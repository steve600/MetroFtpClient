using MetroFtpClient.Core.Base;
using MetroFtpClient.Ftp.Contracts.Interfaces;
using MetroFtpClient.Ftp.FtpClient;
using MetroFtpClient.Ftp.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAPICodePack.Shell;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MetroFtpClient.Ftp.ViewModels
{
    public class FtpClientViewModel : ViewModelBase
    {
        #region Members and Constants

        private FtpConnection ftpConnection = null;

        #endregion Members and Constants

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggrgator">The event aggregator.</param>
        public FtpClientViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            this.Icon = Application.Current.FindResource("appbar_connect");

            // Initialize commands
            this.InitializeCommands();
        }

        #region Initialize

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            this.ProcessQueueCommand = new DelegateCommand(this.ProcessQueue, this.ProcessQueueCanExecute);
            this.StopQueueCommand = new DelegateCommand(this.StopQueue, this.StopQueueCanExecute);
            this.DeleteFtpFileCommand = DelegateCommand.FromAsyncHandler(this.DeleteFtpFile, this.DeleteFtpFileCanExecute);
            this.AddFtpFileToQueueCommand = new DelegateCommand(this.AddFtpFileToQueue, this.AddFtpFileToQueueCanExecute);
            this.OpenFolderInExplorerCommand = new DelegateCommand<IQueueEntry>(this.OpenFolderInExplorer);
            this.StartQueueEntryCommand = new DelegateCommand<object>(this.StartQueueEntry, this.StartQueueEntryCanExecute);
            this.StopQueueEntryCommand = new DelegateCommand<object>(this.StopQueueEntry, this.StopQueueEntryCanExecute);
            this.DeleteQueueEntryCommand = new DelegateCommand<object>(this.DeleteQueueEntry, this.DeleteQueueEntryCanExecute);
        }

        /// <summary>
        /// Initialize connection
        /// </summary>
        /// <param name="connectionSettings">The connection settings</param>
        public async Task InitializeConnection(IConnectionSettings connectionSettings)
        {
            // TODO: Use ConnectionFactory
            this.ftpConnection = await FtpConnection.CreateAsync(connectionSettings);
            this.ftpConnection.PropertyChanged += FtpConnection_PropertyChanged;

            OnPropertyChanged(() => this.Queue);
            OnPropertyChanged(() => this.QueuePending);
            OnPropertyChanged(() => this.QueueFinished);
            OnPropertyChanged(() => this.FtpFilesystem);
        }

        /// <summary>
        /// PropertyChanged-EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FtpConnection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(ftpConnection.FtpLog):
                    this.OnPropertyChanged(() => this.FtpLog);
                    break;
                case nameof(ftpConnection.Queue):
                    this.OnPropertyChanged(() => this.Queue);
                    break;
                case nameof(ftpConnection.QueuePending):
                    this.OnPropertyChanged(() => this.QueuePending);
                    break;
                case nameof(ftpConnection.QueueFinished):
                    this.OnPropertyChanged(() => this.QueueFinished);
                    break;
            }
        }

        #endregion Initialize

        #region Methods

        /// <summary>
        /// Methdod for managing the queue entries
        /// </summary>
        /// <param name="queueEntry">The qeue entry</param>
        /// <param name="method">Method which should be invoked</param>
        private void QueueManagement(object queueEntry, Action<IQueueEntry> method)
        {
            if (queueEntry != null)
            {
                if (queueEntry is System.Collections.IList)
                {
                    System.Collections.IList queueEntries = (System.Collections.IList)queueEntry;

                    foreach (var qe in queueEntries)
                    {
                        method(qe as IQueueEntry);
                    }

                    return;
                }

                if (queueEntry is IQueueEntry)
                    method(queueEntry as IQueueEntry);
            }
        }

        #endregion Methods

        #region Commands

        /// <summary>
        /// The ProcessQueueCommand
        /// </summary>
        public ICommand ProcessQueueCommand { get; private set; }

        /// <summary>
        /// Execute handler for the ProcessQueueCommand
        /// </summary>
        private void ProcessQueue()
        {
            this.ftpConnection.ProcessQueue();
        }

        /// <summary>
        /// CanExecute handler for ProcessQueueCommand
        /// </summary>
        /// <returns></returns>
        private bool ProcessQueueCanExecute()
        {
            //return this.SelectedFtpFilesystemFolder != null;
            return true;
        }

        /// <summary>
        /// The StopQueueCommand
        /// </summary>
        public ICommand StopQueueCommand { get; private set; }

        /// <summary>
        /// Execute handler for the StopQueueCommand
        /// </summary>
        private void StopQueue()
        {
            this.ftpConnection.CancelQueue();
        }

        /// <summary>
        /// CanExecute handler for StopQueueCommand
        /// </summary>
        /// <returns></returns>
        private bool StopQueueCanExecute()
        {
            //return this.SelectedFtpFilesystemFolder != null;
            return true;
        }

        /// <summary>
        /// Add file to queue command
        /// </summary>
        public ICommand AddFtpFileToQueueCommand { get; private set; }

        /// <summary>
        /// Execute handler for AddFtpFileToQueueCommand <see cref="AddFtpFileToQueueCommand"/>
        /// </summary>
        private void AddFtpFileToQueue()
        {            
            this.ftpConnection.AddFtpFileToQueue(this.SelectedFtpFilesystemFolder, this.SelectedDownloadDirectory, Direction.Download);
        }

        /// <summary>
        /// CanExeucte handler for AddFtpFileToQueueCommand <see cref="AddFtpFileToQueueCommand"/>
        /// </summary>
        /// <returns></returns>
        private bool AddFtpFileToQueueCanExecute()
        {
            return this.SelectedFtpFilesystemFolder != null;
        }

        public ICommand DeleteFtpFileCommand { get; private set; }

        /// <summary>
        /// DeleteCommand execute handler
        /// </summary>
        /// <param name="ftpFile">The FTP file</param>
        private async Task DeleteFtpFile()
        {
            await ftpConnection.DeleteFtpFile(this.SelectedFtpFilesystemFolder);
        }

        /// <summary>
        /// DeleteCommand can execute handler
        /// </summary>
        /// <param name="ftpFile">The FTP file</param>
        /// <returns></returns>
        private bool DeleteFtpFileCanExecute()
        {
            return this.SelectedFtpFilesystemFolder != null;
        }

        public ICommand StartQueueEntryCommand { get; private set; }

        /// <summary>
        /// Start a queue entry can execute handler
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        private bool StartQueueEntryCanExecute(object queueEntry)
        {
            return queueEntry != null;
            //if (queueEntry != null)
            //{
            //    return queueEntry.Status == DownloadStatus.Cancelled ||
            //           queueEntry.Status == DownloadStatus.Failed ||
            //           queueEntry.Status == DownloadStatus.Idle ||
            //           queueEntry.Status == DownloadStatus.Paused ||
            //           queueEntry.Status == DownloadStatus.Stopped;
            //}

            //return false;
        }

        /// <summary>
        /// Start a queue entry
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        private void StartQueueEntry(object queueEntry)
        {
            this.QueueManagement(queueEntry, this.ftpConnection.AddEntryToDownloadQueue);
        }

        public ICommand StopQueueEntryCommand { get; private set; }

        /// <summary>
        /// Stop a queue entry
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        private void StopQueueEntry(object queueEntry)
        {
            this.QueueManagement(queueEntry, this.ftpConnection.StopQueueEntry);
        }

        /// <summary>
        /// Stop a queue entry can execute handler
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        private bool StopQueueEntryCanExecute(object queueEntry)
        {
            return queueEntry != null;
            //if (queueEntry != null)
            //{
            //    return queueEntry.Status == DownloadStatus.Transferring ||
            //           queueEntry.Status == DownloadStatus.Waiting;
            //}

            //return false;
        }

        public ICommand DeleteQueueEntryCommand { get; private set; }

        /// <summary>
        /// Delete queue entry command
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        private void DeleteQueueEntry(object queueEntry)
        {
            this.QueueManagement(queueEntry, this.ftpConnection.DeleteQueueEntry);
        }

        /// <summary>
        /// Delete qeue entry command can execute handler
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        /// <returns></returns>
        private bool DeleteQueueEntryCanExecute(object queueEntry)
        {
            return true;
        }

        public ICommand OpenFolderInExplorerCommand { get; private set; }

        /// <summary>
        /// OpenFolderInExplorerCommand execute handler
        /// </summary>
        /// <param name="queueEntry"></param>
        private void OpenFolderInExplorer(IQueueEntry queueEntry)
        {
            if (queueEntry != null)
            {
                var path = System.IO.Path.GetDirectoryName(queueEntry.LocalFile);
                Process.Start(path);
            }
        }

        #endregion Commands

        #region Properties

        /// <summary>
        /// FTP FtpLog-Messages
        /// </summary>
        public string FtpLog
        {
            get { return this.ftpConnection?.FtpLog; }
        }

        /// <summary>
        /// The FTP-Filesystem
        /// </summary>
        public FtpFile FtpFilesystem
        {
            get
            {
                return this.ftpConnection?.FtpFilesystem;
            }
        }

        /// <summary>
        /// Selected FTP folder
        /// </summary>
        public FtpFile SelectedFtpFilesystemFolder
        {
            get { return this.ftpConnection?.SelectedFtpFileSystemFolder; }
            set { this.ftpConnection.SelectedFtpFileSystemFolder = value; }
        }

        /// <summary>
        /// The download queue
        /// </summary>
        public ObservableCollection<IQueueEntry> Queue
        {
            get { return this.ftpConnection?.Queue; }
        }

        /// <summary>
        /// Pending qeueue entries
        /// </summary>
        public IList<IQueueEntry> QueuePending
        {
            get { return this.ftpConnection?.QueuePending; }
        }

        /// <summary>
        /// The finished entries
        /// </summary>
        public IList<IQueueEntry> QueueFinished
        {
            get { return this.ftpConnection?.QueueFinished; }
        }

        private ObservableCollection<ShellObject> selectedExplorerItems;

        /// <summary>
        /// The selected explorer items (e.g. for upload)
        /// </summary>
        public ObservableCollection<ShellObject> SelectedExplorerItems
        {
            get { return selectedExplorerItems; }
            set { this.SetProperty<ObservableCollection<ShellObject>>(ref this.selectedExplorerItems, value); }
        }

        private string selectedDownloadDirectory;

        /// <summary>
        /// The selected donwload directory
        /// </summary>
        public string SelectedDownloadDirectory
        {
            get { return selectedDownloadDirectory; }
            set { this.SetProperty<string>(ref this.selectedDownloadDirectory, value); }
        }

        private IQueueEntry selectedQueueEntry;

        /// <summary>
        /// The selected queue entry
        /// </summary>
        public IQueueEntry SelectedQueueEntry
        {
            get { return selectedQueueEntry; }
            set { this.SetProperty<IQueueEntry>(ref this.selectedQueueEntry, value); }
        }

        #endregion Properties
    }
}