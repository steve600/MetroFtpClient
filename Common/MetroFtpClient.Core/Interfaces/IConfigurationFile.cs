using MetroFtpClient.Core.Configuration;

namespace MetroFtpClient.Core.Interfaces
{
    /// <summary>
    /// <para>
    /// Interface of a configuration file
    /// </para>
    ///
    /// <para>
    /// Class history:
    /// <list type="bullet">
    ///     <item>
    ///         <description>1.0: First release, working (Steffen Steinbrecher).</description>
    ///     </item>
    /// </list>
    /// </para>
    ///
    /// <para>Author: Steffen Steinbrecher</para>
    /// <para>Date: 18.12.2014</para>
    /// </summary>
    public interface IConfigurationFile
    {
        /// <summary>
        /// Method for loading configuration file
        /// </summary>
        /// <returns></returns>
        bool Load();

        /// <summary>
        /// Method for saving configuration file
        /// </summary>
        void Save();

        /// <summary>
        /// The file name
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Sections of the config file
        /// </summary>
        ConfigurationSections Sections { get; set; }
    }
}