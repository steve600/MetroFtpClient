using MetroFtpClient.Ftp.Interfaces;
using System;

namespace MetroFtpClient.Ftp.FtpClient
{
    /// <summary>
    /// The recordString is like
    /// Directory: drwxrwxrwx   1 owner    group               0 Dec 13 11:25 Folder A
    /// File:      -rwxrwxrwx   1 owner    group               1024 Dec 13 11:25 File B
    /// NOTE: The date segment does not contains year.
    /// </summary>
    public class UnixFtpFilesystemParser : IFtpFilesystemParser
    {
        public FtpFile Parse(Uri baseUrl, string recordString)
        {
            //    FtpFile ftpFile = new FtpFile();

            //    ftpFile.OriginalRecordString = recordString.Trim();

            //    // The segments is like "drwxrwxrwx", "",  "", "1", "owner", "", "", "",
            //    // "group", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
            //    // "0", "Dec", "13", "11:25", "Folder", "A".
            //    string[] segments = ftpFile.OriginalRecordString.Split(' ');

            //    int index = 0;

            //    // The permission segment is like "drwxrwxrwx".
            //    string permissionsegment = segments[index];

            //    // If the property start with 'd', then it means a directory.
            //    ftpFile.IsDirectory = permissionsegment[0] == 'd';

            //    // Skip the empty segments.
            //    while (segments[++index] == string.Empty) { }

            //    // Skip the directories segment.

            //    // Skip the empty segments.
            //    while (segments[++index] == string.Empty) { }

            //    // Skip the owner segment.

            //    // Skip the empty segments.
            //    while (segments[++index] == string.Empty) { }

            //    // Skip the group segment.

            //    // Skip the empty segments.
            //    while (segments[++index] == string.Empty) { }

            //    // If this ftpFile is a file, then the fileSize is larger than 0.
            //    ftpFile.FileSize = ulong.Parse(segments[index]);

            //    // Skip the empty segments.
            //    while (segments[++index] == string.Empty) { }

            //    // The month segment.
            //    string monthsegment = segments[index];

            //    // Skip the empty segments.
            //    while (segments[++index] == string.Empty) { }

            //    // The day segment.
            //    string daysegment = segments[index];

            //    // Skip the empty segments.
            //    while (segments[++index] == string.Empty) { }

            //    // The time segment.
            //    string timesegment = segments[index];

            //    ftpFile.ModifiedTime = DateTime.Parse(string.Format("{0} {1} {2} ",
            //        timesegment, monthsegment, daysegment));

            //    // Skip the empty segments.
            //    while (segments[++index] == string.Empty) { }

            //    // Calculate the index of the file name part in the original string.
            //    int filenameIndex = 0;

            //    for (int i = 0; i < index; i++)
            //    {
            //        // "" represents ' ' in the original string.
            //        if (segments[i] == string.Empty)
            //        {
            //            filenameIndex += 1;
            //        }
            //        else
            //        {
            //            filenameIndex += segments[i].Length + 1;
            //        }
            //    }

            //    // The file name may include many segments because the name can contain ' '.
            //    ftpFile.Name = ftpFile.OriginalRecordString.Substring(filenameIndex).Trim();

            //    // Add "/" to the url if it is a directory
            //    ftpFile.Url = new Uri(baseUrl, ftpFile.Name + (ftpFile.IsDirectory ? "/" : string.Empty));

            //    return ftpFile;

            return null;
        }
    }
}