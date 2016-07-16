using MetroFtpClient.Ftp.Contracts.Interfaces;
using MetroFtpClient.Ftp.Events;
using MetroFtpClient.Ftp.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Text;

namespace MetroFtpClient.Ftp.FtpClient
{
    public class FtpConnection : BindableBase
    {
        #region Members and Constants

        // The connection settings
        private IConnectionSettings connectionSettings = null;
        // The FTP-Client
        private FtpClient ftpClient = null;
        // The queue file        
        private string queueFile = string.Empty;
        // The FTP-Log
        private StringBuilder log = new StringBuilder();
        // The download queue
        private DownloadQueue downloadQueue = null;
        private CancellationTokenSource downloadCancellationTokenSource = null;

        #endregion Members and Constants

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="credentials">The credentials.</param>
        private FtpConnection(IConnectionSettings connectionSettings)
        {
            this.connectionSettings = connectionSettings;

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            this.PropertyChanged += FtpConnection_PropertyChanged;
        }

        private void FtpConnection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(this.SelectedFtpFileSystemFolder)))
            {
                if (this.SelectedFtpFileSystemFolder != null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        this.SelectedFtpFileSystemFolder.ChildItems = new ObservableCollection<FtpFile>(this.ftpClient.GetSubDirectoriesAndFiles(this.SelectedFtpFileSystemFolder.Url));
                    });
                }
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns></returns>
        private async Task<FtpConnection> InitializeAsync()
        {
            this.Url = new Uri(connectionSettings.URIString);
            this.ftpClient = new FtpClient(new NetworkCredential(connectionSettings.UserName, connectionSettings.Password));

            this.ftpClient.NewMessageArrived += FtpClient_NewMessageArrived;
            this.ftpClient.FileDownloadCompleted += FtpClient_FtpFileDownloadCompleted;

            // FileName for Queue-Entries
            string fileName = String.Format("{0}_{1}", connectionSettings.Name.Replace(" ", "_"), "Queue.json");
            this.queueFile = Path.Combine(Infrastructure.Constants.FileAndFolderConstants.ApplicationDataFolder, fileName);

            // Load queue entries
            this.LoadQueueEntries();

            // Check whether the Url exists and the credentials is correct.
            // If there is an error, an exception will be thrown.
            if (await this.ftpClient.CheckFTPUrlExist(this.Url))
            {
                // Create ROOT-Entry
                this.FtpFilesystem = new FtpFile(this.Url, "ROOT");
                this.FtpFilesystem.ChildItems.Add(new FtpFile(this.Url, "..", true, true));

                // Set the Status.
                //this.Status = FTPClientManagerStatus.Idle;
            }
            else
            {

            }

            return this;
        }

        /// <summary>
        /// Save queue
        /// </summary>
        private void SaveQueue()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    File.WriteAllText(this.queueFile, JsonConvert.SerializeObject(this.QueuePending, Formatting.Indented));

                    this.OnPropertyChanged(() => this.QueuePending);
                }
                catch(Exception ex)
                {
                    // TODO
                    System.Diagnostics.Debug.Write(ex);
                }
            });
    }

        /// <summary>
        /// Event-Handler for DownloadCompleted-Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FtpClient_FtpFileDownloadCompleted(object sender, FileDownloadCompletedEventArgs e)
        {
            // Get queue entry
            var queueEntry = this.Queue.Where(q => q.RemoteFile.Equals(e.ServerPath.AbsoluteUri) &&
                                                   q.LocalFile.Equals(e.LocalFile.FullName)).First();

            // Remove queue entry
            this.Queue.Remove(queueEntry);

            // Save queue
            this.SaveQueue();

            OnPropertyChanged(() => this.Queue);
            OnPropertyChanged(() => this.QueueFinished);
            OnPropertyChanged(() => this.QueuePending);
        }

        /// <summary>
        /// Event-Handler for NewMessageArrived-Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FtpClient_NewMessageArrived(object sender, NewMessageEventArgs e)
        {
            this.log.Append($"{DateTime.Now.ToLongTimeString()}: {e.NewMessage}");
            this.OnPropertyChanged(() => this.FtpLog);
        }

        /// <summary>
        /// Create async
        /// </summary>
        /// <param name="connectionSettings"></param>
        /// <returns></returns>
        public static Task<FtpConnection> CreateAsync(IConnectionSettings connectionSettings)
        {
            var result = new FtpConnection(connectionSettings);
            return result.InitializeAsync();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Exception);
        }
        
        /// <summary>
        /// Load queue entries
        /// </summary>
        private void LoadQueueEntries()
        {
            if (File.Exists(this.queueFile))
            {
                this.Queue = JsonConvert.DeserializeObject<ObservableCollection<IQueueEntry>>(File.ReadAllText(this.queueFile), new QueueEntryConverter());
            }
        }

        /// <summary>
        /// Intialize download queue
        /// </summary>
        private void InitializeDownloadQueue()
        {
            if (this.downloadQueue == null)
            {
                this.downloadQueue = new DownloadQueue(2);
                this.downloadCancellationTokenSource = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// Process qeue async
        /// </summary>
        public void ProcessQueue()
        {
            InitializeDownloadQueue();

            foreach (var d in this.Queue.Where(q => q.Status == DownloadStatus.Idle || q.Status == DownloadStatus.Stopped))
            {
                d.Status = DownloadStatus.Waiting;
                downloadQueue.EnqueueTask(() => this.ftpClient.DownloadFile(new Uri(d.RemoteFile), d.LocalFile, d.ProgressIndicator, this.downloadCancellationTokenSource.Token), d, this.downloadCancellationTokenSource.Token);
            }            
        }

        /// <summary>
        /// Add entry to download queue
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        public void AddEntryToDownloadQueue(IQueueEntry queueEntry)
        {
            InitializeDownloadQueue();

            this.downloadQueue.EnqueueTask(() => this.ftpClient.DownloadFile(new Uri(queueEntry.RemoteFile), queueEntry.LocalFile, queueEntry.ProgressIndicator, this.downloadCancellationTokenSource.Token), queueEntry, this.downloadCancellationTokenSource.Token);

            queueEntry.Status = DownloadStatus.Waiting;
        }

        /// <summary>
        /// Stop a queue entry
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        public void StopQueueEntry(IQueueEntry queueEntry)
        {
            this.downloadQueue.StopTask(queueEntry);
        }

        /// <summary>
        /// Delete queue entry
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        public void DeleteQueueEntry(IQueueEntry queueEntry)
        {
            this.Queue.Remove(queueEntry);

            // Save queue
            this.SaveQueue();
        }

        /// <summary>
        /// Cancel queue processing
        /// </summary>
        public void CancelQueue()
        {
            if (this.downloadQueue != null && this.downloadCancellationTokenSource != null)
            {
                this.downloadCancellationTokenSource.Cancel();
                this.downloadQueue.Dispose();
                this.downloadQueue = null;
            }
        }

        /// <summary>
        /// Add file to download/upload queue
        /// </summary>
        /// <param name="file">The remote file.</param>
        /// <param name="localBaseDir">The local base dir.</param>
        public void AddFtpFileToQueue(FtpFile file, string localBaseDir, Direction direction)
        {
            Task.Factory.StartNew(() => 
            {
                if (file.IsDirectory)
                {
                    var rootDir = file.Name;

                    foreach (var f in this.ftpClient.GetFileListingForDownload(file.Url))
                    {
                        int startIndex = f.Url.LocalPath.IndexOf(rootDir);
                        var localDirectory = f.Url.LocalPath.Substring(startIndex, f.Url.LocalPath.Length - startIndex);

                        this.Queue.Add(new QueueEntry(NormalizeUri(localBaseDir, localDirectory), f.Url.AbsoluteUri, direction, DownloadStatus.Idle));
                    }
                }
                else
                {
                    var localDirectory = file.Url.LocalPath.Split(new string[] { file.Name }, StringSplitOptions.RemoveEmptyEntries)[1];

                    this.Queue.Add(new QueueEntry(NormalizeUri(localBaseDir, file.Url.LocalPath), file.Url.AbsoluteUri, direction, DownloadStatus.Idle));
                }

                // Save queue file
                this.SaveQueue();              
            });
        }

        /// <summary>
        /// Delete file on server
        /// </summary>
        /// <param name="ftpFile">The FTP file</param>
        /// <returns></returns>
        public async Task DeleteFtpFile(FtpFile ftpFile)
        {
            if (ftpFile.IsDirectory)
                await this.ftpClient.DeleteDirectoryAsync(ftpFile.Url);
            else
                await this.ftpClient.DeleteFileAsync(ftpFile.Url);
        }

        /// <summary>
        /// Normalize URI
        /// </summary>
        /// <param name="uriParams"></param>
        /// <returns></returns>
        public string NormalizeUri(params string[] uriParams)
        {
            var retVal = string.Empty;
            foreach (var uriParam in uriParams)
            {
                var tempParam = uriParam.Trim();
                if (!String.IsNullOrEmpty(tempParam))
                {
                    tempParam = uriParam.Replace('/', '\\').TrimStart('\\');
                    retVal = !String.IsNullOrEmpty(retVal) ? string.Format("{0}\\{1}", retVal, tempParam) : string.Format("{0}", tempParam);
                }
            }
            return retVal;
        }

        #region Properties

        /// <summary>
        /// The current URL of this FTPClient.
        /// </summary>
        public Uri Url { get; private set; }

        private FtpFile ftpFilesystem = null;

        /// <summary>
        /// The FTP-Filesystem
        /// </summary>
        public FtpFile FtpFilesystem
        {
            get
            {
                return this.ftpFilesystem;
            }
            private set { this.SetProperty<FtpFile>(ref this.ftpFilesystem, value); }
        }

        private FtpFile selectedFtpFileSystemFolder;

        /// <summary>
        /// The selected FTP filesystem folder
        /// </summary>
        public FtpFile SelectedFtpFileSystemFolder
        {
            get { return selectedFtpFileSystemFolder; }
            set { this.SetProperty<FtpFile>(ref this.selectedFtpFileSystemFolder, value); }
        }
        
        private ObservableCollection<IQueueEntry> queue = new ObservableCollection<IQueueEntry>();

        /// <summary>
        /// The queue
        /// </summary>
        public ObservableCollection<IQueueEntry> Queue
        {
            get { return queue; }
            private set { this.SetProperty<ObservableCollection<IQueueEntry>>(ref queue, value); }
        }

        private ObservableCollection<IQueueEntry> queueFinished = new ObservableCollection<IQueueEntry>();

        /// <summary>
        /// Finished entries
        /// </summary>
        public IList<IQueueEntry> QueueFinished
        {
            get { return this.Queue.Where(q => q.Status == DownloadStatus.Finished).ToList(); }
        }

        /// <summary>
        /// Pending queue entries
        /// </summary>
        public IList<IQueueEntry> QueuePending
        {
            get { return this.Queue.Where(q => q.Status != DownloadStatus.Finished).ToList(); }
        }

        /// <summary>
        /// The log.
        /// </summary>
        public string FtpLog
        {
            get { return log.ToString(); }
        }

        #endregion Properties
    }
}