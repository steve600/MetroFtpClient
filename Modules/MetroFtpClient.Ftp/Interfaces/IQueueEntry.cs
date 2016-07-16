using MetroFtpClient.Ftp.FtpClient;
using OxyPlot;
using System;

namespace MetroFtpClient.Ftp.Interfaces
{    
    public interface IQueueEntry
    {
        long DownloadSpeedBytesPerSecond { get; set; }
        string DownloadSpeedFormatted { get; }
        string LocalFile { get; set; }
        double PercentCompleted { get; set; }
        IProgress<DownloadProgess> ProgressIndicator { get; }
        string RemoteFile { get; set; }
        DownloadStatus Status { get; set; }
        long TotalBytesRead { get; set; }
        string TotalBytesReadPretty { get; }
        Direction TransferDirection { get; set; }
        PlotModel NetworkSpeedPlot { get; }
    }
}