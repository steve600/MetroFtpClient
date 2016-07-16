namespace MetroFtpClient.Core.Configuration
{
    public class ConfigurationSection
    {
        // Name of the section
        public string Name;

        // ConfigurationSettings for the section
        public ConfigurationSettings Settings;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="sectionName">Name of the section</param>
        public ConfigurationSection(string sectionName)
        {
            this.Name = sectionName;
            this.Settings = new ConfigurationSettings();
        }
    }
}