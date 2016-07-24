using MetroFtpClient.Ftp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetroFtpClient.Ftp.FtpClient
{
    /// <summary>
    /// Class that represents a download task
    /// </summary>
    public class Download
    {
        public readonly TaskCompletionSource<object> TaskSource;
        public readonly Action Action;
        public readonly CancellationToken? CancelToken;
        public readonly IQueueEntry QueueEntry;

        /// <summary>
        /// CTOR 
        /// </summary>
        /// <param name="taskSource">The task completion source.</param>
        /// <param name="queueEntry">The queue entry.</param>
        /// <param name="action">The download action.</param>
        /// <param name="cancelToken">The cancellation token</param>
        public Download(TaskCompletionSource<object> taskSource, IQueueEntry queueEntry, Action action, CancellationToken? cancelToken)
        {
            TaskSource = taskSource;
            QueueEntry = queueEntry;
            Action = action;
            CancelToken = cancelToken;
        }
    }

    /// <summary>
    /// Download queue
    /// </summary>
    public class DownloadQueue : IDisposable
    {
        // BlockingCollection with downloads
        private BlockingCollection<Download> downloads = new BlockingCollection<Download>();
        // List with the running tasks
        private readonly IList<Task> runningsTasks = null;

        /// <summary>
        /// CTOR for a download queue
        /// </summary>
        /// <param name="workerCount"></param>
        public DownloadQueue(int workerCount)
        {
            runningsTasks = new List<Task>();

            // Create and start a separate Task for each consumer
            for (int i = 0; i < workerCount; i++)
            {
                var t = Task.Factory.StartNew(Consume);

                runningsTasks.Add(t);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            downloads.CompleteAdding();
        }

        /// <summary>
        /// Enqueu a download task
        /// </summary>
        /// <param name="action">The download action.</param>
        /// <param name="queueEntry">The queue entry.</param>
        /// <returns></returns>
        public Task EnqueueTask(Action action, IQueueEntry queueEntry)
        {
            return EnqueueTask(action, queueEntry, null);
        }

        /// <summary>
        /// Enqeue a download task
        /// </summary>
        /// <param name="action">The download action.</param>
        /// <param name="queueEntry">The queue entry.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        /// <returns></returns>
        public Task EnqueueTask(Action action, IQueueEntry queueEntry, CancellationToken? cancelToken)
        {
            var tcs = new TaskCompletionSource<object>();
            downloads.Add(new Download(tcs, queueEntry, action, cancelToken));
            return tcs.Task;
        }

        /// <summary>
        /// Stop download task
        /// </summary>
        /// <param name="queueEntry">The queue entry.</param>
        public void StopTask(IQueueEntry queueEntry)
        {
            var d = downloads.Where(dl => dl.QueueEntry.LocalFile == queueEntry.LocalFile &&
                                          dl.QueueEntry.RemoteFile == queueEntry.RemoteFile).FirstOrDefault();

            if (d != null)
            {
                d.TaskSource.SetCanceled();
            }
        }

        /// <summary>
        /// Consume
        /// </summary>
        private void Consume()
        {
            foreach (Download download in downloads.GetConsumingEnumerable())
            {
                if (download.CancelToken.HasValue &&
                    download.CancelToken.Value.IsCancellationRequested)
                {
                    download.TaskSource.SetCanceled();
                    download.QueueEntry.Status = DownloadStatus.Stopped;
                }
                else
                {
                    try
                    {
                        // Start download
                        download.Action();
                        download.TaskSource.SetResult(null);   // Indicate completion
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (ex.CancellationToken == download.CancelToken)
                            download.TaskSource.SetCanceled();
                        else
                            download.TaskSource.SetException(ex);
                    }
                    catch (Exception ex)
                    {
                        download.TaskSource.SetException(ex);
                    }
                }
            }
        }
    }
}