using MetroFtpClient.Ftp.Interfaces;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MetroFtpClient.Ftp.FtpClient
{
    /// <summary>
    /// Class for parsing UNIX file and directory informations using the FTP LIST method
    /// Parts are taken from: https://github.com/bentonstark/starksoft-aspen
    /// </summary>
    public class UnixFtpFilesystemParser : IFtpFilesystemParser
    {
        // unix regex expressions
        private Regex isUnix = new Regex(@"(d|l|-|b|c|p|s)(r|w|x|-|t|s){9}", RegexOptions.Compiled);
        private Regex unixAttribs = new Regex(@"(d|l|-|b|c|p|s)(r|w|x|-|t|s){9}", RegexOptions.Compiled);
        private Regex unixMonth = new Regex(@"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|mrt|mei|okt)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex unixDay = new Regex(@"(?<=(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|mrt|mei|okt)\s+)\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex unixYear = new Regex(@"(?<=(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|mrt|mei|okt)\s+\d+\s+)(19|20)\d\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex unixTime = new Regex(@"(?<=(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|mrt|mei|okt)\s+\d+\s+)\d+:\d\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex unixSize = new Regex(@"\d+(?=(\s+(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|mrt|mei|okt)))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex unixName = new Regex(@"((?<=((Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|mrt|mei|okt)\s+\d+\s+(19|20)\d\d\s+)|((Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|mrt|mei|okt)\s+\d+\s+\d+:\d\d\s+)).+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex unixSymbLink = new Regex(@"(?<=\s+->\s+).+", RegexOptions.Compiled);
        private Regex unixType = new Regex(@"(d|l|-|b|c|p|s)(?=(r|w|x|-|t|s){9})", RegexOptions.Compiled);

        /// <summary>
        /// Parse directory information
        /// </summary>
        /// <param name="baseUrl">The base URL</param>
        /// <param name="recordString">The record string.</param>
        /// <returns></returns>
        public FtpFile Parse(Uri baseUrl, string recordString)
        {
            // Instantiate the regular expression object.
            string attribs = unixAttribs.Match(recordString).ToString();
            string month = unixMonth.Match(recordString).ToString();
            string day = unixDay.Match(recordString).ToString();
            string year = unixYear.Match(recordString).ToString();
            string time = unixTime.Match(recordString).ToString();
            string size = unixSize.Match(recordString).ToString();
            string name = unixName.Match(recordString).ToString().Trim();
            string symbLink = "";

            // ignore the microsoft 'etc' file that IIS uses for WWW users
            if (name == "~ftpsvc~.ckm")
                return null;

            //  if we find a symbolic link then extract the symbolic link first and then
            //  extract the file name portion
            if (unixSymbLink.IsMatch(name))
            {
                symbLink = unixSymbLink.Match(name).ToString();
                name = name.Substring(0, name.IndexOf("->")).Trim();
            }

            string itemType = unixType.Match(recordString).ToString();


            //  if the current year is not given in unix then we need to figure it out.
            //  basically, if a date is within the past 6 months unix will show the 
            //  time instead of the year
            if (year.Length == 0)
            {
                int curMonth = DateTime.Today.Month;
                int curYear = DateTime.Today.Year;

                DateTime result;
                if (DateTime.TryParse(String.Format(CultureInfo.InvariantCulture, "1-{0}-2007", month), out result))
                {
                    if ((curMonth - result.Month) < 0)
                        year = Convert.ToString(curYear - 1, CultureInfo.InvariantCulture);
                    else
                        year = curYear.ToString(CultureInfo.InvariantCulture);
                }
            }

            DateTime dateObj;
            DateTime.TryParse(String.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2} {3}", day, month, year, time), out dateObj);

            ulong sizeLng = 0;
            ulong.TryParse(size, out sizeLng);

            FtpItemType itemTypeObj = FtpItemType.Unknown;
            switch (itemType.ToLower(CultureInfo.InvariantCulture))
            {
                case "l":
                    itemTypeObj = FtpItemType.SymbolicLink;
                    break;
                case "d":
                    itemTypeObj = FtpItemType.Directory;
                    break;
                case "-":
                    itemTypeObj = FtpItemType.File;
                    break;
                case "b":
                    itemTypeObj = FtpItemType.BlockSpecialFile;
                    break;
                case "c":
                    itemTypeObj = FtpItemType.CharacterSpecialFile;
                    break;
                case "p":
                    itemTypeObj = FtpItemType.NamedSocket;
                    break;
                case "s":
                    itemTypeObj = FtpItemType.DomainSocket;
                    break;
            }

            if (itemTypeObj == FtpItemType.Unknown || name.Trim().Length == 0)
                return null;
            else
                return new FtpFile(recordString, name, dateObj, sizeLng, symbLink, attribs, itemTypeObj, new Uri(baseUrl, name + ((itemTypeObj == FtpItemType.Directory) ? "/" : string.Empty)));
        }
    }
}