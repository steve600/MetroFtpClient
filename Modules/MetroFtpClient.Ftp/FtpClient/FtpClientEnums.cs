using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroFtpClient.Ftp.FtpClient
{
    public enum Direction
    {
        Download,
        Upload
    }

    public enum DownloadStatus
    {
        Idle,
        Waiting,
        Transferring,
        Finished,
        Stopped,
        Paused,
        Cancelled,
        Failed
    }
}
