using MetroFtpClient.Core.ExtensionMethods;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MetroFtpClient.Ftp.FtpClient
{
    /// <summary>
    /// The itemType of item as reported by the FTP server.
    /// </summary>
    /// <remarks>
    /// Data transmitted from the FTP server after a directory list operation is usually a item itemType of Directory or File.  Unix 
    /// systems also support additional directory item types such as symbolic links and named sockets.  Not all FTP servers will report
    /// enough information to determine the file itemType.  In such cases a file itemType of Unknown is specified.
    /// </remarks>
    public enum FtpItemType
    {
        /// <summary>
        /// Directory item.
        /// </summary>
        Directory,
        /// <summary>
        /// File item.
        /// </summary>
        File,
        /// <summary>
        /// Symbolic link item.
        /// </summary>
        SymbolicLink,
        /// <summary>
        /// Block special file item.
        /// </summary>
        BlockSpecialFile,
        /// <summary>
        /// Character special file item.
        /// </summary>
        CharacterSpecialFile,
        /// <summary>
        /// Name socket item.
        /// </summary>
        NamedSocket,
        /// <summary>
        /// Domain socket item.
        /// </summary>
        DomainSocket,
        /// <summary>
        /// Unknown item.  The system was unable to determine the itemType of item.
        /// </summary>
        Unknown
    }

    public class FtpFile : BindableBase
    {
        /// <summary>
        /// Standard CTOR
        /// </summary>
        public FtpFile(Uri baseDir, string name, bool isExpanded = false, bool isSelected = false)
        {
            this.Url = baseDir;
            this.Name = name;
            this.IsExpanded = isExpanded;
            this.IsSelected = isSelected;
        }

        /// <summary>
        /// The FtpFile class represents the file and directory listing items as reported by the FTP server.
        /// </summary>
        /// <param name="originalRecordString">The original record string.</param>
        /// <param name="name">The name of the file/directory.</param>
        /// <param name="modifiedTime">Modified date and/or time of the item.</param>
        /// <param name="fileSize">Number of bytes or size of the item.</param>
        /// <param name="symbolicLink">Symbolic link name.</param>
        /// <param name="attributes">Permissions for the item</param>
        /// <param name="ftpItemType">The type of the item.</param>
        /// <param name="url">The url.</param>
        public FtpFile(string originalRecordString, string name, DateTime modifiedTime, ulong fileSize, string symbolicLink, string attributes, FtpItemType ftpItemType, Uri url)
        {
            this.OriginalRecordString = originalRecordString;
            this.Name = name;
            this.ModifiedTime = modifiedTime;
            this.FileSize = fileSize;
            this.SymbolicLink = symbolicLink;
            this.Attributes = attributes;
            this.ItemType = ftpItemType;
            this.Url = url;
        }

        private bool isExpanded;

        /// <summary>
        /// Flag if node is expanded
        /// </summary>
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { this.SetProperty<bool>(ref this.isExpanded, value); }
        }

        private bool isSelected;

        /// <summary>
        /// Flag if node is selected
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { this.SetProperty<bool>(ref this.isSelected, value); }
        }


        private string originalRecordString;

        /// <summary>
        /// The original record string.
        /// </summary>
        public string OriginalRecordString
        {
            get { return originalRecordString; }
            set { this.SetProperty<string>(ref this.originalRecordString, value); }
        }

        private Uri url;

        /// <summary>
        /// The server Path.
        /// </summary>
        public Uri Url
        {
            get { return url; }
            set { this.SetProperty<Uri>(ref this.url, value); }
        }

        private FtpItemType itemType;

        /// <summary>
        /// The item type <see cref="FtpItemType"/>
        /// </summary>
        public FtpItemType ItemType
        {
            get { return itemType; }
            set { this.SetProperty<FtpItemType>(ref this.itemType, value); }
        }


        private string name;

        /// <summary>
        /// The name of this FTPFileSystem instance.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { this.SetProperty<string>(ref this.name, value); }
        }

        private string attributes;

        public string Attributes
        {
            get { return attributes; }
            set { this.SetProperty<string>(ref this.attributes, value); }
        }
        

        /// <summary>
        /// Specify whether this FTPFileSystem instance is a directory.
        /// </summary>
        public bool IsDirectory
        {
            get { return this.ItemType == FtpItemType.Directory; }
        }

        private string symbolicLink;

        /// <summary>
        /// The filesystem link
        /// </summary>
        public string SymbolicLink
        {
            get { return symbolicLink; }
            set { this.SetProperty<string>(ref this.symbolicLink, value); }
        }

        private bool isLoaded = false;

        /// <summary>
        /// Specify whether this FTPFileSystem instance loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { this.SetProperty<bool>(ref this.isLoaded, value); }
        }

        private DateTime modifiedTime;

        /// <summary>
        /// The last modified time of this FTPFileSystem instance.
        /// </summary>
        public DateTime ModifiedTime
        {
            get { return modifiedTime; }
            set { this.SetProperty<DateTime>(ref this.modifiedTime, value); }
        }

        private ulong fileSize;

        /// <summary>
        /// The fileSize of this FTPFileSystem instance if it is not a directory.
        /// </summary>
        public ulong FileSize
        {
            get { return fileSize; }
            set
            {
                if (this.SetProperty<ulong>(ref this.fileSize, value))
                {
                    OnPropertyChanged(() => FileSizePretty);
                }
            }
        }

        /// <summary>
        /// Pretty file size
        /// </summary>
        public string FileSizePretty
        {
            get
            {
                return this.FileSize.ToPrettySize(2);
            }
        }

        private string user;

        /// <summary>
        /// The user
        /// </summary>
        public string User
        {
            get { return user; }
            set { this.SetProperty<string>(ref this.user, value); }
        }

        private string group;

        /// <summary>
        /// The group
        /// </summary>
        public string Group
        {
            get { return group; }
            set { this.SetProperty<string>(ref this.group, value); }
        }

        private FtpFile parentItem;

        /// <summary>
        /// Parent item
        /// </summary>
        public FtpFile ParentItem
        {
            get { return parentItem; }
            set { this.SetProperty<FtpFile>(ref this.parentItem, value); }
        }

        private ObservableCollection<FtpFile> childItems = new ObservableCollection<FtpFile>();

        /// <summary>
        /// Child items
        /// </summary>
        public ObservableCollection<FtpFile> ChildItems
        {
            get { return childItems; }
            set
            {
                if (this.SetProperty<ObservableCollection<FtpFile>>(ref this.childItems, value))
                {
                    OnPropertyChanged(() => this.Folders);
                    OnPropertyChanged(() => this.Files);
                }
            }
        }

        /// <summary>
        /// List with folders
        /// </summary>
        public ObservableCollection<FtpFile> Folders
        {
            get
            {
                return new ObservableCollection<FtpFile>(this.ChildItems.Where(i => i.ItemType == FtpItemType.Directory));
            }
        }

        /// <summary>
        /// List with files
        /// </summary>
        public ObservableCollection<FtpFile> Files
        {
            get
            {
                return new ObservableCollection<FtpFile>(this.ChildItems.Where(i => i.ItemType != FtpItemType.Directory));
            }
        }
    }
}