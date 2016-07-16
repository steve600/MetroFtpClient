using MetroFtpClient.Ftp.Contracts.Interfaces;

namespace MetroFtpClient.Ftp.FtpClient
{
    /// <summary>
    /// Factory for creating the different connection types
    /// </summary>
    public class ConnectionFactory : IConnectionFactory
    {
        public IConnectionSettings GetConnection(ConnectionTypes connectionType)
        {
            IConnectionSettings result = null;

            switch (connectionType)
            {
                case ConnectionTypes.FTP:
                    result = new FtpConnectionSettings();
                    break;

                default:
                    result = new FtpConnectionSettings();
                    break;
            }

            return result;
        }
    }
}