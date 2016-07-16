namespace MetroFtpClient.Ftp.Contracts.Interfaces
{
    /// <summary>
    /// Interface for a connection factory
    /// </summary>
    public interface IConnectionFactory
    {
        IConnectionSettings GetConnection(ConnectionTypes connectionType);
    }
}