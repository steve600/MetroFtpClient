using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace MetroFtpClient.SystemInfo
{
    public static class DotNetFrameworkInfo
    {
        /// <summary>
        /// Get list of installed framework versions
        /// </summary>
        /// <returns></returns>
        public static IList<NetFrameworkVersionInfo> InstalledDotNetVersions()
        {
            IList<NetFrameworkVersionInfo> versions = new List<NetFrameworkVersionInfo>();
            RegistryKey NDPKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
            if (NDPKey != null)
            {
                string[] subkeys = NDPKey.GetSubKeyNames();
                foreach (string subkey in subkeys)
                {
                    GetDotNetVersion(NDPKey.OpenSubKey(subkey), subkey, versions);
                    if (subkey == "v4")
                    {
                        GetDotNetVersion(NDPKey.OpenSubKey(subkey).OpenSubKey("Client"), subkey, versions);
                        GetDotNetVersion(NDPKey.OpenSubKey(subkey).OpenSubKey("Full"), subkey, versions);
                    }
                }
            }

            return versions;
        }

        /// <summary>
        /// Get framework version for specified SubKey
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="subVersionName"></param>
        /// <param name="versions"></param>
        private static void GetDotNetVersion(RegistryKey parentKey, string subVersionName, IList<NetFrameworkVersionInfo> versions)
        {
            if (parentKey != null)
            {
                string installed = Convert.ToString(parentKey.GetValue("Install"));

                if (installed == "1")
                {
                    NetFrameworkVersionInfo versionInfo = new NetFrameworkVersionInfo();

                    versionInfo.VersionString = Convert.ToString(parentKey.GetValue("Version"));

                    var test = versionInfo.BaseVersion;

                    versionInfo.InstallPath = Convert.ToString(parentKey.GetValue("InstallPath"));
                    versionInfo.ServicePackLevel = Convert.ToInt32(parentKey.GetValue("SP"));

                    if (parentKey.Name.Contains("Client"))
                        versionInfo.FrameworkProfile = "Client Profile";
                    else if (parentKey.Name.Contains("Full"))
                        versionInfo.FrameworkProfile = "Full Profile";

                    if (!versions.Contains(versionInfo))
                        versions.Add(versionInfo);
                }
            }
        }

        /// <summary>
        /// Class to persist Framework version information
        /// </summary>
        public class NetFrameworkVersionInfo
        {
            /// <summary>
            /// Base-Version, e.g. .NET Framework 2.0
            /// </summary>
            public string BaseVersion
            {
                get
                {
                    string version = string.Empty;

                    if (Convert.ToInt32(VersionString.Substring(2, 1)) <= 4)
                        version = VersionString.Substring(0, 2) + "0";
                    else
                        version = VersionString.Substring(0, 2) + "5";

                    if (!String.IsNullOrEmpty(FrameworkProfile))
                        version += " " + FrameworkProfile;

                    return ".NET Framework " + version;
                }
            }

            /// <summary>
            /// Complete Version-String, e.g. 2.2.257
            /// </summary>
            public string VersionString { get; set; }

            /// <summary>
            /// Servicepack-Level, e.g. 2
            /// </summary>
            public int ServicePackLevel { get; set; }

            /// <summary>
            /// Installation path
            /// </summary>
            public string InstallPath { get; set; }

            /// <summary>
            /// Framework-Profile, e.g. Client or Full
            /// </summary>
            public string FrameworkProfile { get; set; }

            /// <summary>
            /// Complete .NET Framework version string, .NET Framework 2.2.257 Service-Pack 2
            /// </summary>
            public string Name
            {
                get
                {
                    return ".NET Framework " + this.VersionString + " " + this.ServicePackString + this.FrameworkProfile;
                }
            }

            /// <summary>
            /// Serice Pack string
            /// </summary>
            public string ServicePackString
            {
                get
                {
                    if (ServicePackLevel > 0)
                        return "Service Pack " + ServicePackLevel.ToString();
                    else
                        return string.Empty;
                }
            }
        }
    }
}