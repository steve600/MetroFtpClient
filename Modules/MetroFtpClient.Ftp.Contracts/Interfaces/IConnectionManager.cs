using System.Collections.ObjectModel;

namespace MetroFtpClient.Ftp.Contracts.Interfaces
{
    public interface IConnectionManager
    {
        /// <summary>
        /// The active connection
        /// </summary>
        IConnectionSettings ActiveConnection { get; set; }

        /// <summary>
        /// List with available connections
        /// </summary>
        ObservableCollection<IConnectionSettings> Connections { get; }

        /// <summary>
        /// Create a connection
        /// </summary>
        /// <param name="connectionKind">The connection kind</param>
        /// <param name="connectionType">The connection type</param>
        /// <returns></returns>
        IConnectionSettings CreateConnection(ConnectionTypes connectionType);

        /// <summary>
        /// Add a new connection
        /// </summary>
        /// <param name="connection">The new connection</param>
        /// <returns></returns>
        bool AddConnection(IConnectionSettings connection);
    }
}