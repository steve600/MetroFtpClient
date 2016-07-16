using System;

namespace MetroFtpClient.Infrastructure.Constants
{
    public static class FileAndFolderConstants
    {
        /// <summary>
        /// The application data folder
        /// </summary>
        public static string ApplicationDataFolder
        {
            get
            {
                string basePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MetroFtpClient");

                // Create directory if not exists
                if (!System.IO.Directory.Exists(basePath))
                    System.IO.Directory.CreateDirectory(basePath);

                return basePath;
            }
        }

        /// <summary>
        /// Application config file
        /// </summary>
        public static string ApplicationConfigFile
        {
            get
            {
                return System.IO.Path.Combine(ApplicationDataFolder, "ApplicationSettings.xml");
            }
        }

        /// <summary>
        /// Connections settings config file
        /// </summary>
        public static string ConnectionSettingsConfigFile
        {
            get
            {
                return System.IO.Path.Combine(ApplicationDataFolder, "ConnectionSettings.xml");
            }
        }

        /// <summary>
        /// Folder with the log files
        /// </summary>
        public static string LogFileFolder
        {
            get
            {
                return System.IO.Path.Combine(ApplicationDataFolder, "Logs");
            }
        }
    }
}