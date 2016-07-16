using MetroFtpClient.Ftp.Events;
using MetroFtpClient.Ftp.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetroFtpClient.Core.ExtensionMethods;

namespace MetroFtpClient.Ftp.FtpClient
{
    /// <summary>
    /// Class to report the download progress
    /// </summary>
    public class DownloadProgess
    {
        public double Percent { get; set; }
        public long TotalBytesDownloaded { get; set; }
        public DownloadStatus DownloadStatus { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public long DownloadSpeedBytesPerSecond { get; set; }
    }

    /// <summary>
    /// Class for the FTP client functions
    /// </summary>
    public class FtpClient
    {
        #region Members and Constants

        private NetworkCredential credentials;
        private const int uiUpdateIntervall = 1000;

        #endregion Members and Constants

        #region Events

        //public event EventHandler<FtpErrorEventArgs> FtpErrorOccurred;
        //public event EventHandler StatusChanged;

        public event EventHandler<FileDownloadCompletedEventArgs> FileDownloadCompleted;

        public event EventHandler<NewMessageEventArgs> NewMessageArrived;

        /// <summary>
        /// Fire new message arrived event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewMessageArrived(NewMessageEventArgs e)
        {
            NewMessageArrived?.Invoke(this, e);
        }

        protected virtual void OnFileDownloadCompleted(FileDownloadCompletedEventArgs e)
        {
            FileDownloadCompleted?.Invoke(this, e);
        }

        #endregion Events

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="credentials">The network credentials.</param>
        public FtpClient(NetworkCredential credentials)
        {
            this.credentials = credentials;
        }

        /// <summary>
        /// Get file size
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns></returns>
        public long GetFileSize(Uri url)
        {
            long fileSize = 0;

            FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.GetFileSize);

            try
            {
                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    fileSize = response.ContentLength;
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ex.Response is FtpWebResponse)
                {
                    OnNewMessageArrived(new NewMessageEventArgs(((FtpWebResponse)ex.Response).StatusDescription));
                }
            }

            return fileSize;
        }

        /// <summary>
        /// Get file size
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns></returns>
        public async Task<long> GetFileSizeAsync(Uri url)
        {
            long fileSize = 0;

            FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.GetFileSize);

            try
            {
                using (FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse)
                {
                    fileSize = response.ContentLength;
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ex.Response is FtpWebResponse)
                {
                    OnNewMessageArrived(new NewMessageEventArgs(((FtpWebResponse)ex.Response).StatusDescription));
                }
            }

            return fileSize;
        }

        /// <summary>
        /// Get the sub directories and files of the Url. It will be used in enumerate
        /// all the folders.
        /// When run the FTP LIST protocol method to get a detailed listing of the files
        /// on an FTP server, the server will response many records of information. Each
        /// record represents a file.
        /// </summary>
        public IList<FtpFile> GetSubDirectoriesAndFiles(Uri url)
        {
            OnNewMessageArrived(new NewMessageEventArgs(String.Format("[R] Getting directories and files for {0}{1}", url.AbsoluteUri, Environment.NewLine)));

            IList<FtpFile> subDirs = new List<FtpFile>();

            FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.ListDirectoryDetails);

            try
            {
                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    this.OnNewMessageArrived(new NewMessageEventArgs(response.StatusDescription));

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string subDir = reader.ReadLine();

                            IFtpFilesystemParser parser = null;

                            // Find out the FTP Directory Listing Style from the recordString.
                            if (!string.IsNullOrEmpty(subDir))
                            {
                                parser = FtpFilesystemParserFactory.Get(subDir);
                            }
                            while (!string.IsNullOrEmpty(subDir))
                            {
                                subDirs.Add(parser.Parse(url, subDir));

                                subDir = reader.ReadLine();
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ex.Response is FtpWebResponse)
                {
                    OnNewMessageArrived(new NewMessageEventArgs(((FtpWebResponse)ex.Response).StatusDescription));
                }
            }            

            return subDirs;
        }

        /// <summary>
        /// Create directory
        /// </summary>
        /// <param name="url">The URI</param>
        /// <returns></returns>
        public async Task CreateDirectoryAsync(Uri url)
        {
            try
            {
                FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.MakeDirectory);
                
                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    Stream data = response.GetResponseStream();
                }

                System.Diagnostics.Debug.WriteLine($"Directory {url.AbsoluteUri} created");

            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Directory creation cancelled!");
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine($"Creation of directory {url.AbsoluteUri} failed!");
                System.Diagnostics.Debug.WriteLine(e);
                //throw;
            }
        }

        /// <summary>
        /// Get the sub directories and files of the Url. It will be used in enumerate
        /// all the folders.
        /// When run the FTP LIST protocol method to get a detailed listing of the files
        /// on an FTP server, the server will response many records of information. Each
        /// record represents a file.
        /// </summary>
        public async Task<IList<FtpFile>> GetSubDirectoriesAndFilesAsync(Uri url)
        {
            OnNewMessageArrived(new NewMessageEventArgs(String.Format("[R] Getting directories and files for {0}{1}", url.AbsoluteUri, Environment.NewLine)));

            IList<FtpFile> subDirs = new List<FtpFile>();

            FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.ListDirectoryDetails);
            
            try
            {
                using (FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse)
                {
                    this.OnNewMessageArrived(new NewMessageEventArgs(response.StatusDescription));

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string subDir = reader.ReadLine();

                            IFtpFilesystemParser parser = null;

                            // Find out the FTP Directory Listing Style from the recordString.
                            if (!string.IsNullOrEmpty(subDir))
                            {
                                parser = FtpFilesystemParserFactory.Get(subDir);
                            }
                            while (!string.IsNullOrEmpty(subDir))
                            {
                                subDirs.Add(parser.Parse(url, subDir));

                                subDir = reader.ReadLine();
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ex.Response is FtpWebResponse)
                {
                    OnNewMessageArrived(new NewMessageEventArgs(((FtpWebResponse)ex.Response).StatusDescription));
                }
            }            

            return subDirs;
        }

        /// <summary>
        /// Get file listing for download
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public IList<FtpFile> GetFileListingForDownload(Uri url)
        {
            IList<FtpFile> result = new List<FtpFile>();

            var files = this.GetSubDirectoriesAndFiles(url);

            foreach (var f in files)
            {
                if (f.IsDirectory)
                {
                    foreach(var sf in this.GetFileListingForDownload(f.Url))
                    {
                        result.Add(sf);
                    }
                }
                else
                {
                    result.Add(f);
                }
            }

            return result;
        }

        /// <summary>
        /// Download file asyn
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="targetPath">The target path</param>
        /// <param name="progress">Progess changed handler</param>
        /// <param name="token">The cancellation tocken.</param>
        /// <returns></returns>
        public void DownloadFile(Uri url, string localFile, IProgress<DownloadProgess> progress, CancellationToken cancelToken)
        {
            int bytesRead = default(int);
            long totalBytesRead = default(long);
            double percent = default(double);
            long fileSize = default(long);

            Stopwatch sw = new Stopwatch();
            long lastUpdatedTime = default(long);
            long lastUpdateDownloadedSize = default(long);

            try
            {
                // Check local dir
                this.CheckTargetDir(Path.GetDirectoryName(localFile));

                // Get file size from server
                fileSize = GetFileSize(url);

                FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.DownloadFile);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    sw.Start();
                    lastUpdatedTime = sw.ElapsedMilliseconds;

                    using (Stream data = response.GetResponseStream())
                    {
                        System.Diagnostics.Debug.WriteLine($"Downloading {url.AbsoluteUri} to {localFile}...");

                        byte[] byteBuffer = new byte[8 * 1024];
                        using (FileStream output = new FileStream(localFile, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                                                                    FileShare.ReadWrite, 8 * 1024, useAsync: true))
                        {
                            totalBytesRead = output.Length;

                            do
                            {
                                bytesRead = data.Read(byteBuffer, 0, byteBuffer.Length);

                                if (bytesRead > 0)
                                {
                                    totalBytesRead += bytesRead;

                                    if (progress != null)
                                    {
                                        // Report progress
                                        if (sw.ElapsedMilliseconds - lastUpdatedTime >= uiUpdateIntervall)
                                        {
                                            //System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Updated UI");
                                            DownloadProgess args = new DownloadProgess();
                                            args.DownloadStatus = DownloadStatus.Transferring;
                                            args.Percent = (totalBytesRead <= 0) ? default(double) : ((double)totalBytesRead / (double)fileSize);
                                            args.TotalBytesDownloaded = totalBytesRead;
                                            args.ElapsedTime = sw.Elapsed;

                                            // Calculate speed (Bytes per Second)
                                            long sizeDiff = totalBytesRead - lastUpdateDownloadedSize;
                                            lastUpdateDownloadedSize = totalBytesRead;
                                            args.DownloadSpeedBytesPerSecond = sizeDiff / TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds - lastUpdatedTime).Seconds;

                                            // Set last updated time
                                            lastUpdatedTime = sw.ElapsedMilliseconds;

                                            // Report progress
                                            progress.Report(args);
                                        }
                                    }

                                    output.Write(byteBuffer, 0, bytesRead);
                                }
                            }
                            while (bytesRead > 0 && !cancelToken.IsCancellationRequested);
                        }

                        sw.Stop();

                        var downloadProgress = new DownloadProgess() { Percent = percent, TotalBytesDownloaded = totalBytesRead, ElapsedTime = sw.Elapsed };

                        if (cancelToken.IsCancellationRequested)
                        {
                            downloadProgress.DownloadStatus = DownloadStatus.Cancelled;
                        }
                        else
                        {
                            downloadProgress.DownloadStatus = DownloadStatus.Finished;
                            this.OnFileDownloadCompleted(new FileDownloadCompletedEventArgs(url, new FileInfo(localFile)));
                        }

                        progress.Report(downloadProgress);
                                                
                        System.Diagnostics.Debug.WriteLine($"Downloaded {url.AbsoluteUri} to {localFile}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Download cancelled!");
                progress.Report(new DownloadProgess() { Percent = percent, TotalBytesDownloaded = totalBytesRead, DownloadStatus = DownloadStatus.Stopped });
            }
            catch (WebException e)
            {
                progress.Report(new DownloadProgess() { Percent = percent, TotalBytesDownloaded = totalBytesRead, DownloadStatus = DownloadStatus.Failed });
                System.Diagnostics.Debug.WriteLine($"Failed to download {url.AbsoluteUri} to {localFile}");
                System.Diagnostics.Debug.WriteLine(e);
                //throw;
            }
            finally
            {
                sw.Stop();
                sw.Reset();
            }
        }

        /// <summary>
        /// Download file asyn
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="targetPath">The target path</param>
        /// <param name="progress">Progess changed handler</param>
        /// <param name="token">The cancellation tocken.</param>
        /// <returns></returns>
        public async Task DownloadFileAsync(Uri url, string localFile, IProgress<DownloadProgess> progress, CancellationToken cancellationToken)
        {
            int bytesRead = default(int);
            long totalBytesRead = default(long);
            double percent = default(double);

            Stopwatch sw = new Stopwatch();
            long start = default(long);

            try
            {
                // Check local dir
                this.CheckTargetDir(Path.GetDirectoryName(localFile));

                // Get file size from server
                long fileSize = await GetFileSizeAsync(url);

                FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.DownloadFile);

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    sw.Start();
                    start = sw.ElapsedMilliseconds;

                    Stream data = response.GetResponseStream();

                    System.Diagnostics.Debug.WriteLine($"Downloading {url.AbsoluteUri} to {localFile}...");

                    byte[] byteBuffer = new byte[8 * 1024];
                    using (FileStream output = new FileStream(localFile, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                                                                FileShare.ReadWrite, 8 * 1024, useAsync: true))
                    {
                        if (File.Exists(localFile))
                        {
                            var fi = new FileInfo(localFile);
                            totalBytesRead = fi.Length;
                        }

                        do
                        {
                            bytesRead = await data.ReadAsync(byteBuffer, 0, byteBuffer.Length);


                            if (bytesRead > 0)
                            {
                                // Report progress
                                totalBytesRead += bytesRead;
                                if (totalBytesRead > 0)
                                {
                                    if (progress != null)
                                    {
                                        if (sw.ElapsedMilliseconds - start >= uiUpdateIntervall)
                                        {
                                            start = sw.ElapsedMilliseconds;
                                            DownloadProgess args = new DownloadProgess();
                                            args.DownloadStatus = DownloadStatus.Transferring;
                                            args.Percent = (totalBytesRead <= 0) ? default(double) : ((double)totalBytesRead / (double)fileSize);
                                            args.TotalBytesDownloaded = totalBytesRead;
                                            progress.Report(args);
                                        }
                                    }
                                }

                                await output.WriteAsync(byteBuffer, 0, bytesRead, cancellationToken);
                            }
                        }
                        while (bytesRead > 0);
                    }

                    sw.Stop();
                    sw.Reset();
                    progress.Report(new DownloadProgess() { Percent = percent, TotalBytesDownloaded = totalBytesRead, DownloadStatus = DownloadStatus.Finished });
                    System.Diagnostics.Debug.WriteLine($"Downloaded {url.AbsoluteUri} to {localFile}");
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Download cancelled!");
                progress.Report(new DownloadProgess() { Percent = percent, TotalBytesDownloaded = totalBytesRead, DownloadStatus = DownloadStatus.Stopped });
            }
            catch (WebException e)
            {
                progress.Report(new DownloadProgess() { Percent = percent, TotalBytesDownloaded = totalBytesRead, DownloadStatus = DownloadStatus.Failed });
                System.Diagnostics.Debug.WriteLine($"Failed to download {url.AbsoluteUri} to {localFile}");
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            finally
            {
                sw.Stop();
                sw.Reset();
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="url">The URI.</param>
        /// <returns>Returns <c>true</c> if operation is successfull; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteFileAsync(Uri url)
        {
            bool result = true;

            try
            {
                FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.DeleteFile);

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    Stream data = response.GetResponseStream();
                }

                System.Diagnostics.Debug.WriteLine($"Deleted {url.AbsoluteUri}");

            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Delete file cancelled!");
                result = false;
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete {url.AbsoluteUri}");
                System.Diagnostics.Debug.WriteLine(e);
                result = false;
                //throw;
            }

            return result;
        }

        /// <summary>
        /// Delete directory
        /// </summary>
        /// <param name="url">The URI.</param>
        /// <returns>Returns <c>true</c> if operation is successfull; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteDirectoryAsync(Uri url)
        {
            bool result = true;

            try
            {
                var files = await this.GetSubDirectoriesAndFilesAsync(url);

                foreach (var f in files)
                {
                    if (f.IsDirectory)
                        await DeleteDirectoryAsync(f.Url);
                    else
                        await DeleteFileAsync(f.Url);
                }

                FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.RemoveDirectory);

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    Stream data = response.GetResponseStream();
                    response.Close();
                }

                System.Diagnostics.Debug.WriteLine($"Deleted {url.AbsoluteUri}");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Delete directory cancelled!");
                result = false;
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete {url.AbsoluteUri}");
                System.Diagnostics.Debug.WriteLine(e);
                result = false;
                //throw;
            }

            return result;
        }

        /// <summary>
        /// If the Url does not exist, an exception will be thrown.
        /// </summary>
        public async Task<bool> CheckFTPUrlExist(Uri url)
        {
            bool urlExist = await VerifyFTPUrlExistAsync(url);

            if (!urlExist)
            {
                throw new ApplicationException("The url does not exist");
            }

            return urlExist;
        }

        /// <summary>
        /// Verify whether the url exists.
        /// </summary>
        private async Task<bool> VerifyFTPUrlExistAsync(Uri url)
        {
            bool urlExist = false;

            if (url.IsFile)
            {
                urlExist = await VerifyFileExistAsync(url);
            }
            else
            {
                urlExist = await VerifyDirectoryExistAsync(url);
            }

            return urlExist;
        }

        /// <summary>
        /// Verify whether the directory exists.
        /// </summary>
        private async Task<bool> VerifyDirectoryExistAsync(Uri url)
        {
            FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.ListDirectory);

            try
            {
                using (FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse)
                {

                    // Messages
                    this.OnNewMessageArrived(new NewMessageEventArgs(response.BannerMessage));
                    this.OnNewMessageArrived(new NewMessageEventArgs(response.WelcomeMessage));
                    this.OnNewMessageArrived(new NewMessageEventArgs(response.StatusDescription));

                    //return response.StatusCode == FtpStatusCode.DataAlreadyOpen;
                    return true;
                }
            }
            catch (System.Net.WebException webEx)
            {
                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                if (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }

                throw;
            }            
        }

        /// <summary>
        /// Verify whether the file exists.
        /// </summary>
        private async Task<bool> VerifyFileExistAsync(Uri url)
        {
            FtpWebRequest request = this.CreateFtpWebRequest(url, WebRequestMethods.Ftp.GetFileSize);

            try
            {
                using (FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse)
                {

                    return response.StatusCode == FtpStatusCode.FileStatus;
                }
            }
            catch (System.Net.WebException webEx)
            {
                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                if (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        /// Create a FtpWebRequest
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestMethod">The reqest method <see cref="WebRequestMethods.Ftp"/> </param>
        /// <returns></returns>
        private FtpWebRequest CreateFtpWebRequest(Uri url, string requestMethod)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url), "The URL is null.");

            if (url.Scheme != Uri.UriSchemeFtp)
                throw new ArgumentException("The request URI isn't a valid FTP URI", nameof(url));

            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;
            request.Credentials = this.credentials;

            request.Method = requestMethod;

            return request;
        }

        /// <summary>
        /// Check local local directory (if not exists -> try to create the directory)
        /// </summary>
        /// <param name="targetDir">The directory path.</param>
        private void CheckTargetDir(string targetDir)
        {
            DirectoryInfo di = new DirectoryInfo(targetDir);

            if (!di.Exists)
            {
                di.Create();
            }
        }
    }
}