using MetroFtpClient.Ftp.Contracts.Interfaces;
using Prism.Mvvm;
using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MetroFtpClient.Ftp.FtpClient
{
    public abstract class ConnectionSettingsBase : BindableBase, IConnectionSettings, ICloneable
    {
        /// <summary>
        /// CTOR
        /// </summary>
        protected ConnectionSettingsBase()
        {
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="name">The connection name</param>
        /// <param name="host">The host</param>
        /// <param name="port">The port</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password</param>
        /// <param name="timeout">Timeout</param>
        protected ConnectionSettingsBase(string name, string host, int port, string userName, string password, int timeout = 10000)
        {
            this.Name = name;
            this.Host = host;
            this.Port = port;
            this.UserName = userName;
            this.Password = password;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Create from XElement
        /// </summary>
        /// <param name="settings"></param>
        protected ConnectionSettingsBase(XElement settings)
        {
            this.LoadFromXml(settings);
        }

        /// <summary>
        /// ToXml-Method
        /// </summary>
        /// <returns></returns>
        public virtual XElement ToXml()
        {
            return new XElement("Connection",
                        new XAttribute(nameof(this.ConnectionType), this.ConnectionType),
                        new XAttribute(nameof(this.Name), this.Name),
                        new XAttribute(nameof(this.Host), this.Host),
                        new XAttribute(nameof(this.Port), this.Port),
                        new XAttribute(nameof(this.UserName), this.UserName),
                        new XAttribute(nameof(this.Password), this.Password),
                        new XAttribute(nameof(this.Timeout), this.Timeout));
        }

        /// <summary>
        /// Load from xml
        /// </summary>
        /// <param name="xml"></param>
        public virtual void LoadFromXml(XElement settings)
        {
            this.Name = settings.Attribute(nameof(this.Name))?.Value;
            this.Host = settings.Attribute(nameof(this.Host))?.Value;
            this.Port = (settings.Attribute(nameof(this.Port))?.Value != null) ? Convert.ToInt32(settings.Attribute(nameof(this.Port))?.Value) : default(int);
            this.UserName = settings.Attribute(nameof(this.UserName))?.Value;
            this.Password = settings.Attribute(nameof(this.Password))?.Value;
            this.Timeout = (settings.Attribute(nameof(this.Timeout))?.Value != null) ? Convert.ToInt32(settings.Attribute(nameof(this.Timeout))?.Value) : 10000;
        }

        /// <summary>
        /// The connection type
        /// </summary>
        public abstract ConnectionTypes ConnectionType { get; }

        /// <summary>
        /// The URI-String
        /// </summary>
        public abstract string URIString { get; }


        private string name;

        /// <summary>
        /// Name
        /// </summary>
        [XmlAttribute(nameof(Name))]
        public string Name
        {
            get { return name; }
            set { this.SetProperty<string>(ref this.name, value); }
        }

        private string host;

        /// <summary>
        /// Host
        /// </summary>
        [XmlAttribute(nameof(Host))]
        public string Host
        {
            get { return host; }
            set { this.SetProperty<string>(ref this.host, value); }
        }

        private int port;

        /// <summary>
        /// The port
        /// </summary>
        [XmlAttribute(nameof(Port), DataType = "Int32")]
        public int Port
        {
            get { return port; }
            set { this.SetProperty<int>(ref this.port, value); }
        }

        private string userName;

        /// <summary>
        /// UserName
        /// </summary>
        [XmlAttribute(nameof(UserName))]
        public string UserName
        {
            get { return userName; }
            set { this.SetProperty<string>(ref this.userName, value); }
        }

        private string password;

        /// <summary>
        /// Password
        /// </summary>
        [XmlAttribute(nameof(Password))]
        public string Password
        {
            get { return password; }
            set { this.SetProperty<string>(ref this.password, value); }
        }

        private int timeout;

        /// <summary>
        /// Timeout
        /// </summary>
        [XmlAttribute(nameof(Timeout), DataType = "Int32")]
        public int Timeout
        {
            get { return timeout; }
            set { this.SetProperty<int>(ref this.timeout, value); }
        }

        #region Interface ICloneable

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion Interface ICloneable
    }
}