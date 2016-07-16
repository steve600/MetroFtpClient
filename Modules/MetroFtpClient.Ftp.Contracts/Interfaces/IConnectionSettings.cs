using System.Xml.Linq;

namespace MetroFtpClient.Ftp.Contracts.Interfaces
{
    /// <summary>
    /// The connection type
    /// </summary>
    public enum ConnectionTypes
    {
        FTP
    }

    /// <summary>
    /// Interface for the connection
    /// </summary>
    public interface IConnectionSettings
    {
        /// <summary>
        /// ToXml-Method
        /// </summary>
        /// <returns></returns>
        XElement ToXml();

        /// <summary>
        /// Load from xml
        /// </summary>
        /// <param name="xml"></param>
        void LoadFromXml(XElement xml);

        /// <summary>
        /// Connection type
        /// </summary>
        ConnectionTypes ConnectionType { get; }

        /// <summary>
        /// The URI-String
        /// </summary>
        string URIString { get; }

        /// <summary>
        /// Connection name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The host
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// The port
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// The user name
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// The passwort
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Timeout
        /// </summary>
        int Timeout { get; set; }
    }
}