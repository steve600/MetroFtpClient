using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MetroFtpClient.SystemInfo
{
    public enum OfficeVersions
    {
        Office95,
        Office97,
        Office2000,
        Office2002,
        Office2003,
        Office2007,
        Office2010
    }

    public class OfficeVersionInfo
    {
        private const string OUTLOOK_INST_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE";
        private const string OUTLOOK_BITNESS = @"SOFTWARE\Microsoft\Office\%VERSION%\Outlook";

        public string DisplayName { get; set; }
        public string VersionString { get; set; }
        public string OfficeVersion { get; set; }
        public string bitness { get; set; }

        public FileVersionInfo OutlookVersionInfo { get; private set; }

        public string InstalledOutlookVersion
        {
            get
            {
                return "Outlook " + this.OfficeVersion + " " + this.bitness + " (" + this.OutlookVersionInfo.FileVersion + ")";
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="versionString"></param>
        /// <param name="officeVersion"></param>
        public OfficeVersionInfo(string displayName, string versionString, string officeVersion)
        {
            this.DisplayName = displayName;
            this.VersionString = versionString;
            this.OfficeVersion = officeVersion;
        }

        /// <summary>
        /// Get Outlook Version Info
        /// </summary>
        /// <param name="regKey"></param>
        public void GetOutlookVersionInfo(string regKey)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(OUTLOOK_INST_PATH);
            var installationFolder = key.GetValue("Path").ToString();

            var s = Path.Combine(installationFolder, "OUTLOOK.exe");

            var fi = new FileInfo(s);

            if (fi.Exists)
            {
                this.OutlookVersionInfo = FileVersionInfo.GetVersionInfo(fi.FullName);
            }

            // Get Bitness
            key = Registry.LocalMachine.OpenSubKey(OUTLOOK_BITNESS.Replace("%VERSION%", regKey));
            this.bitness = key.GetValue("Bitness").ToString();
        }
    }

    public class OfficeInfo
    {
        #region Members and Constants

        private const string OFFICE_REG_PATH_32_BIT = @"SOFTWARE\Microsoft\Office\";
        private const string OFFICE_REG_PATH_64_BIT = @"SOFTWARE\Wow6432Node\Microsoft\Office\";

        private const string OUTLOOK_INST_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE";
        private const string OUTLOOK_BITNESS = @"SOFTWARE\Microsoft\Office\%VERSION%\Outlook";

        private Dictionary<string, string> officeVersions = new Dictionary<string, string>();
        private List<string> installedOfficeVersions = new List<string>();

        private List<OfficeVersionInfo> supportedOfficeVersions = new List<OfficeVersionInfo>();

        #endregion Members and Constants

        /// <summary>
        /// Ctor
        /// </summary>
        public OfficeInfo()
        {
            this.supportedOfficeVersions.Add(new OfficeVersionInfo("Microsoft® Office 2010", "14.0", "2010"));
            this.supportedOfficeVersions.Add(new OfficeVersionInfo("Microsoft® Office 2007", "12.0", "2007"));

            officeVersions.Add("7.0", "95");
            officeVersions.Add("8.0", "97");
            officeVersions.Add("9.0", "2000");
            officeVersions.Add("10.0", "2002");
            officeVersions.Add("11.0", "2003");
            officeVersions.Add("12.0", "2007");
            officeVersions.Add("14.0", "2010");

            this.GetInstalledOfficeVersions();
        }

        /// <summary>
        /// Get installed office versions
        /// </summary>
        /// <returns></returns>
        private void GetInstalledOfficeVersions()
        {
            string regKey = string.Empty;

            if (OSVersionInfo.OSBits == OSVersionInfo.SoftwareArchitecture.Bit64)
            {
                regKey = OFFICE_REG_PATH_64_BIT;
            }
            else if (OSVersionInfo.OSBits == OSVersionInfo.SoftwareArchitecture.Bit32)
            {
                regKey = OFFICE_REG_PATH_32_BIT;
            }
            else
            {
                // Show error message
            }

            RegistryKey officeKey = Registry.LocalMachine.OpenSubKey(regKey);
            if (officeKey != null)
            {
                string[] subkeys = officeKey.GetSubKeyNames();
                foreach (string subkey in subkeys)
                {
                    var result = this.supportedOfficeVersions.Where(v => v.VersionString.Equals(subkey)).FirstOrDefault();

                    if (result != null)
                    {
                        result.GetOutlookVersionInfo(subkey);
                        this.InstalledOfficeVersion = result;
                    }
                }
            }
        }

        /// <summary>
        /// Supported office versions
        /// </summary>
        public List<OfficeVersionInfo> SupportedOfficeVersions
        {
            get
            {
                return this.supportedOfficeVersions;
            }
        }

        /// <summary>
        /// Installed Office-Version
        /// </summary>
        public OfficeVersionInfo InstalledOfficeVersion
        {
            get;
            private set;
        }
    }
}