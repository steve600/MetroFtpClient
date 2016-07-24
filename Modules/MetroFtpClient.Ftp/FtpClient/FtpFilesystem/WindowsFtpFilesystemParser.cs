using MetroFtpClient.Ftp.Interfaces;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MetroFtpClient.Ftp.FtpClient
{
    /// <summary>
    /// Class for parsing DOS/Windows file and directory informations using the FTP LIST method
    /// 12-13-10  12:41PM       <DIR>          Folder A
    /// Parts are taken from: https://github.com/bentonstark/starksoft-aspen
    /// </summary>
    public class WindowsFtpFilesystemParser : IFtpFilesystemParser
    {
        private Regex dosName = new System.Text.RegularExpressions.Regex(@"((?<=<DIR>\s+).+)|((?<=\d\d:\d\d\s+).+)|((?<=(\d\d:\d\d(AM|PM|am|pm)\s+\d+\s+)).+)", RegexOptions.Compiled);
        private Regex dosDate = new System.Text.RegularExpressions.Regex(@"(\d\d-\d\d-\d\d)", RegexOptions.Compiled);
        private Regex dosTime = new System.Text.RegularExpressions.Regex(@"(\d\d:\d\d\s*(AM|PM|am|pm))|(\d\d:\d\d)", RegexOptions.Compiled);
        private Regex dosSize = new System.Text.RegularExpressions.Regex(@"((?<=(\d\d:\d\d\s*(AM|PM|am|pm)\s*))\d+)|(\d+(?=\s+\d\d-\d\d-\d\d\s+))", RegexOptions.Compiled);
        private Regex dosDir = new System.Text.RegularExpressions.Regex(@"<DIR>|\sDIR\s", RegexOptions.Compiled);

        /// <summary>
        /// Parse directory/file information
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="recordString"></param>
        /// <returns></returns>
        public FtpFile Parse(Uri baseUrl, string recordString)
        {
            string name = dosName.Match(recordString).ToString().Trim();

            // if the name has no length the simply stop processing and return null.
            if (name.Trim().Length == 0)
                return null;

            string date = dosDate.Match(recordString).ToString();
            string time = dosTime.Match(recordString).ToString();
            string size = dosSize.Match(recordString).ToString();
            string dir = dosDir.Match(recordString).ToString().Trim();

            // put togther the date/time
            DateTime dateTime = DateTime.MinValue;
            DateTime.TryParse(String.Format(CultureInfo.InvariantCulture, "{0} {1}", date, time), out dateTime);

            // parse the file size
            ulong sizeLng = 0;
            ulong.TryParse(size, out sizeLng);

            // determine the file item itemType
            FtpItemType itemTypeObj;
            if (dir.Length > 0)
                itemTypeObj = FtpItemType.Directory;
            else
                itemTypeObj = FtpItemType.File;

            //if (name == ".")
            //    itemTypeObj = FtpItemType.;
            //if (name == "..")
            //    itemTypeObj = FtpItemType.ParentDirectory;

            return new FtpFile(recordString, name, dateTime, sizeLng, String.Empty, String.Empty, itemTypeObj, baseUrl);

        }
    }
}