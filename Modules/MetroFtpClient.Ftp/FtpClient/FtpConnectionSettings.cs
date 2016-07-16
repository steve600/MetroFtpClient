using MetroFtpClient.Ftp.Contracts.Interfaces;
using System;

namespace MetroFtpClient.Ftp.FtpClient
{
    public class FtpConnectionSettings : ConnectionSettingsBase
    {
        public override ConnectionTypes ConnectionType
        {
            get
            {
                return ConnectionTypes.FTP;
            }
        }

        public override string URIString
        {
            get
            {
                //return String.Format("ftp://{0}:{1}@{2}", this.UserName, this.Password, this.Host);
                return String.Format("ftp://{0}", this.Host);
            }
        }
    }
}