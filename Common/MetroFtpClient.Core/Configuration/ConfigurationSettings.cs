using System;
using System.Collections;

namespace MetroFtpClient.Core.Configuration
{
    /// <summary>
    /// Configuration settings
    /// </summary>
    public class ConfigurationSettings : DictionaryBase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigurationSetting this[string name]
        {
            set { this.Dictionary[name] = value; }
            get { return (ConfigurationSetting)this.Dictionary[name]; }
        }

        /// <summary>
        /// Add method
        /// </summary>
        /// <param name="settingName">Setting name</param>
        /// <param name="defaultValue">Default value</param>
        public void Add(string settingName, string defaultValue)
        {
            this.Dictionary.Add(settingName, new ConfigurationSetting(settingName, defaultValue));
        }

        /// <summary>
        /// Add setting
        /// </summary>
        /// <param name="settingName">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="value">The value.</param>
        public void Add(string settingName, string defaultValue, string value, Type dataType)
        {
            this.Dictionary.Add(settingName, new ConfigurationSetting(settingName, defaultValue, value, dataType));
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return (Dictionary.Contains(key));
        }

        /// <summary>
        /// Remove entry
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            this.Dictionary.Remove(name);
        }

        /// <summary>
        /// Values
        /// </summary>
        public ICollection Values
        {
            get { return Dictionary.Values; }
        }

        /// <summary>
        /// Key
        /// </summary>
        public ICollection Keys
        {
            get { return Dictionary.Keys; }
        }
    }
}