using MetroFtpClient.Ftp.FtpClient;
using System;

namespace MetroFtpClient.Ftp.Interfaces
{
    public interface IFtpFilesystemParser
    {
        FtpFile Parse(Uri baseUrl, string recordString);
    }
}