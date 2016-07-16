using MetroFtpClient.Ftp.Interfaces;
using System.Text.RegularExpressions;

namespace MetroFtpClient.Ftp.FtpClient
{
    public static class FtpFilesystemParserFactory
    {
        public static IFtpFilesystemParser Get(string recordString)
        {
            Regex regex = new System.Text.RegularExpressions.Regex(@"^([d-]|[l-]|[c-])([r-][w-][x-]){3}$");

            string header = recordString.Substring(0, 10);

            // If the style is UNIX, then the header is like "drwxrwxrwx".
            if (regex.IsMatch(header))
            {
                return new UnixFtpFileSystemParserRegex();
            }
            else
            {
                return new WindowsFtpFilesystemParser();
            }
        }
    }
}