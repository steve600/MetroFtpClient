using MetroFtpClient.Core.ExtensionMethods;
using MetroFtpClient.Ftp.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Mvvm;
using System;
using System.Linq;

namespace MetroFtpClient.Ftp.FtpClient
{
    public class QueueEntryConverter : CustomCreationConverter<IQueueEntry>
    {
        public override IQueueEntry Create(Type objectType)
        {
            return new QueueEntry();
        }
    }

    /// <summary>
    /// Class that represents a queue entry
    /// </summary>
    public class QueueEntry : BindableBase, IQueueEntry
    {
        #region Members and Constants

        private DateTime time;
        private bool networkSpeedInitialized = false;

        #endregion Members and Constants

        /// <summary>
        /// Standard CTOR
        /// </summary>
        public QueueEntry()
        {
            this.ProgressIndicator = new Progress<DownloadProgess>(ReportProgress);

            // Create network speed plot
            this.NetworkSpeedPlot = new PlotModel();
            this.NetworkSpeedPlot.Title = "Speed";            
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="localFile">The local file</param>
        /// <param name="remoteFile">The remote file </param>
        /// <param name="transferDirection">The transfer direction <see cref="Direction"/></param>
        /// <param name="status">The queue status <see cref="DownloadStatus"/></param>
        [JsonConstructor]
        public QueueEntry(string localFile, string remoteFile, Direction transferDirection, DownloadStatus status = DownloadStatus.Idle) :
            this()
        {
            this.LocalFile = localFile;
            this.RemoteFile = remoteFile;
            this.TransferDirection = transerDirection;
            this.Status = status;            
        }

        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="progress"></param>
        private void ReportProgress(DownloadProgess progress)
        {
            this.TotalBytesRead = progress.TotalBytesDownloaded;
            this.PercentCompleted = progress.Percent;
            if (this.Status != progress.DownloadStatus)
            {
                this.Status = progress.DownloadStatus;
            }
            this.DownloadSpeedBytesPerSecond = progress.DownloadSpeedBytesPerSecond;

            if (!networkSpeedInitialized)
                this.InitializeNetworkSpeedPlot();

            this.UpdateNetworkSpeedPlot();
        }

        /// <summary>
        /// Initialize network speed plot
        /// </summary>
        private void InitializeNetworkSpeedPlot()
        {
            this.time = DateTime.Now;

            // Add Y-axis
            this.NetworkSpeedPlot.Axes.Add(new LinearAxis()
            {
                IsZoomEnabled = false,
                Minimum = 0,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left,
            });

            // Add X-axis
            this.NetworkSpeedPlot.Axes.Add(new DateTimeAxis()
            {
                IsZoomEnabled = false,
                Position = AxisPosition.Bottom,
                IsAxisVisible = false
            });

            // Create line series to visualize the values
            var areaSeries = new AreaSeries()
            {
                StrokeThickness = 1,
                LineStyle = OxyPlot.LineStyle.Solid,
                Color = OxyColors.Blue
            };

            // Fill series with initial values
            for (int i = 0; i < 60; i++)
            {
                areaSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time.Subtract(new TimeSpan(0, 0, 60 - i))), 0));
            }

            // Add to plot
            this.NetworkSpeedPlot.Series.Add(areaSeries);

            this.networkSpeedInitialized = true;
        }

        /// <summary>
        /// Update network speed plot
        /// </summary>
        private void UpdateNetworkSpeedPlot()
        {
            var areaSeries = (AreaSeries)this.NetworkSpeedPlot.Series[0];

            if (areaSeries.Points.Count > 60)
            {
                areaSeries.Points.RemoveAt(0);
            }

            areaSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(this.time), this.DownloadSpeedBytesPerSecond));
            areaSeries.Points2.Add(DateTimeAxis.CreateDataPoint(this.time, 0));

            var speedAxe = NetworkSpeedPlot.Axes.Where(a => a is LinearAxis).FirstOrDefault();

            if (speedAxe != null)
            {
                speedAxe.Title = this.DownloadSpeedFormatted;
            }

            time = time.AddSeconds(1);

            this.NetworkSpeedPlot.InvalidatePlot(true);
        }

        private string localFile;

        /// <summary>
        /// The local path
        /// </summary>
        [JsonProperty]
        public string LocalFile
        {
            get { return localFile; }
            set { this.SetProperty<string>(ref this.localFile, value); }
        }

        private Direction transerDirection;

        /// <summary>
        /// Transfer direction <see cref="Direction"/>
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Direction TransferDirection
        {
            get { return transerDirection; }
            set { this.SetProperty<Direction>(ref this.transerDirection, value); }
        }

        private DownloadStatus status;

        /// <summary>
        /// The status <see cref="DownloadStatus"/>
        /// </summary>
        //[JsonProperty]
        //[JsonConverter(typeof(StringEnumConverter))]
        [JsonIgnore]
        public DownloadStatus Status
        {
            get { return status; }
            set { this.SetProperty<DownloadStatus>(ref this.status, value); }
        }

        private string remoteFile;

        /// <summary>
        /// Remote file path
        /// </summary>
        [JsonProperty]
        public string RemoteFile
        {
            get { return remoteFile; }
            set { this.SetProperty<string>(ref this.remoteFile, value); }
        }

        private IProgress<DownloadProgess> progressIndicator;

        /// <summary>
        /// Progress indicator
        /// </summary>
        public IProgress<DownloadProgess> ProgressIndicator
        {
            get { return progressIndicator; }
            private set { this.SetProperty<IProgress<DownloadProgess>>(ref this.progressIndicator, value); }
        }

        private long totalBytesRead;

        /// <summary>
        /// Total bytes read
        /// </summary>
        [JsonIgnore]
        public long TotalBytesRead
        {
            get { return totalBytesRead; }
            set
            {
                if (this.SetProperty<long>(ref this.totalBytesRead, value))
                {
                    OnPropertyChanged(() => this.TotalBytesReadPretty);
                }
            }
        }

        /// <summary>
        /// Total bytes read formatted
        /// </summary>
        [JsonIgnore]
        public string TotalBytesReadPretty
        {
            get
            {
                return this.TotalBytesRead.ToPrettySize();
            }
        }

        private double percentCompleted;

        /// <summary>
        /// Percent completed
        /// </summary>
        [JsonIgnore]
        public double PercentCompleted
        {
            get { return percentCompleted; }
            set { this.SetProperty<double>(ref this.percentCompleted, value); }
        }

        private long downloadSpeedBytesPerSecond;

        /// <summary>
        /// The download speed in bytes per second
        /// </summary>
        [JsonIgnore]
        public long DownloadSpeedBytesPerSecond
        {
            get { return downloadSpeedBytesPerSecond; }
            set
            {
                if (this.SetProperty<long>(ref this.downloadSpeedBytesPerSecond, value))
                {
                    OnPropertyChanged(() => this.DownloadSpeedFormatted);
                }
            }
        }

        /// <summary>
        /// Formatted download speed, e.g. 1.82 MB\s
        /// </summary>
        [JsonIgnore]
        public string DownloadSpeedFormatted
        {
            get
            {
                return $"{DownloadSpeedBytesPerSecond.ToPrettySize(2)}\\s";
            }
        }

        private PlotModel networkSpeedPlot;

        /// <summary>
        /// Network speed plot
        /// </summary>
        [JsonIgnore]
        public PlotModel NetworkSpeedPlot
        {
            get { return networkSpeedPlot; }
            private set { this.SetProperty<PlotModel>(ref this.networkSpeedPlot, value); }
        }
    }
}