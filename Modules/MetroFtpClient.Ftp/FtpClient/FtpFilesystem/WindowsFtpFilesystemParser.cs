using MetroFtpClient.Ftp.Interfaces;
using System;

namespace MetroFtpClient.Ftp.FtpClient
{
    /// <summary>
    /// 12-13-10  12:41PM       <DIR>          Folder A
    /// </summary>
    /// <param name="recordString"></param>
    /// <returns></returns>
    public class WindowsFtpFilesystemParser : IFtpFilesystemParser
    {
        public FtpFile Parse(Uri baseUrl, string recordString)
        {
            //FtpFile ftpFile = new FtpFile();

            //ftpFile.OriginalRecordString = recordString.Trim();

            //// The segments is like "12-13-10",  "", "12:41PM", "", "","", "",
            //// "", "", "<DIR>", "", "", "", "", "", "", "", "", "", "Folder", "A".
            //string[] segments = ftpFile.OriginalRecordString.Split(' ');

            //int index = 0;

            //// The date segment is like "12-13-10" instead of "12-13-2010" if Four-digit years
            //// is not checked in IIS.
            //string dateSegment = segments[index];
            //string[] dateSegments = dateSegment.Split(new char[] { '-' },
            //    StringSplitOptions.RemoveEmptyEntries);

            //int month = int.Parse(dateSegments[0]);
            //int day = int.Parse(dateSegments[1]);
            //int year = int.Parse(dateSegments[2]);

            //// If year >=50 and year <100, then  it means the year 19**
            //if (year >= 50 && year < 100)
            //{
            //    year += 1900;
            //}

            //// If year <50, then it means the year 20**
            //else if (year < 50)
            //{
            //    year += 2000;
            //}

            //// Skip the empty segments.
            //while (segments[++index] == string.Empty) { }

            //// The time segment.
            //string timesegment = segments[index];

            //ftpFile.ModifiedTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}",
            //    year, month, day, timesegment));

            //// Skip the empty segments.
            //while (segments[++index] == string.Empty) { }

            //// The fileSize or directory segment.
            //// If this segment is "<DIR>", then it means a directory, else it means the
            //// file fileSize.
            //string sizeOrDirSegment = segments[index];

            //ftpFile.IsDirectory = sizeOrDirSegment.Equals("<DIR>",
            //    StringComparison.OrdinalIgnoreCase);

            //// If this ftpFile is a file, then the fileSize is larger than 0.
            //if (!ftpFile.IsDirectory)
            //{
            //    ftpFile.FileSize = ulong.Parse(sizeOrDirSegment);
            //}

            //// Skip the empty segments.
            //while (segments[++index] == string.Empty) { }

            //// Calculate the index of the file name part in the original string.
            //int filenameIndex = 0;

            //for (int i = 0; i < index; i++)
            //{
            //    // "" represents ' ' in the original string.
            //    if (segments[i] == string.Empty)
            //    {
            //        filenameIndex += 1;
            //    }
            //    else
            //    {
            //        filenameIndex += segments[i].Length + 1;
            //    }
            //}
            //// The file name may include many segments because the name can contain ' '.
            //ftpFile.Name = ftpFile.OriginalRecordString.Substring(filenameIndex).Trim();

            //// Add "/" to the url if it is a directory
            //ftpFile.Url = new Uri(baseUrl, ftpFile.Name + (ftpFile.IsDirectory ? "/" : string.Empty));

            //return ftpFile;

            return null;
        }
    }
}