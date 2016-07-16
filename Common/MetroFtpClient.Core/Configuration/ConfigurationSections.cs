using System.Collections;

namespace MetroFtpClient.Core.Configuration
{
    public class ConfigurationSections : DictionaryBase
    {
        public ConfigurationSection this[string name]
        {
            set { this.Dictionary[name] = value; }
            get { return (ConfigurationSection)this.Dictionary[name]; }
        }

        /// <summary>
        /// Add-method
        /// </summary>
        /// <param name="name"></param>
        public void Add(string name)
        {
            this.Dictionary.Add(name, new ConfigurationSection(name));
        }

        public void Remove(string name)
        {
            this.Dictionary.Remove(name);
        }

        public ICollection Values
        {
            get { return Dictionary.Values; }
        }

        public ICollection Keys
        {
            get { return Dictionary.Keys; }
        }
    }
}