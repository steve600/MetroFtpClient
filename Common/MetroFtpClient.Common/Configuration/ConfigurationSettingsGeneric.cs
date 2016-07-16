using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroFtpClient.Core.Configuration
{
    public class ConfigurationSettingsGeneric<Setting> : IDictionary<string, Setting>
    {
        private Dictionary<string, Setting> internalDict = new Dictionary<string, Setting>();

        #region IDictionary<string,Setting> Members

        public void Add(string key, Setting value)
        {
            this.internalDict.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return internalDict.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return internalDict.Keys; }
        }

        public bool Remove(string key)
        {
            return internalDict.Remove(key);
        }

        public bool TryGetValue(string key, out Setting value)
        {
            return internalDict.TryGetValue(key, out value);
        }

        public ICollection<Setting> Values
        {
            get { return this.internalDict.Values; }
        }

        public Setting this[string key]
        {
            get
            {
                return this.internalDict[key];
            }
            set
            {
                internalDict[key] = value;
            }
        }

        #endregion IDictionary<string,Setting> Members

        #region ICollection<KeyValuePair<string,Setting>> Members

        public void Add(KeyValuePair<string, Setting> item)
        {
            this.internalDict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.internalDict.Clear();
        }

        public bool Contains(KeyValuePair<string, Setting> item)
        {
            return this.internalDict.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Setting>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return this.internalDict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, Setting> item)
        {
            return this.internalDict.Remove(item.Key);
        }

        #endregion ICollection<KeyValuePair<string,Setting>> Members

        #region IEnumerable<KeyValuePair<string,Setting>> Members

        public IEnumerator<KeyValuePair<string, Setting>> GetEnumerator()
        {
            return this.internalDict.GetEnumerator();
        }

        #endregion IEnumerable<KeyValuePair<string,Setting>> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.internalDict.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}