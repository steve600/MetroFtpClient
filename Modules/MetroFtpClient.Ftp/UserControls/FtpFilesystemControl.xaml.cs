using MetroFtpClient.Ftp.Contracts.Events;
using MetroFtpClient.Ftp.FtpClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace MetroFtpClient.Ftp.UserControls
{
    /// <summary>
    /// Interaktionslogik für FtpFilesystemControl.xaml
    /// </summary>
    public partial class FtpFilesystemControl : UserControl
    {
        public FtpFilesystemControl()
        {
            InitializeComponent();
        }

        private void SetItemInEditMode(EditableTextBlock item, bool EditMode)
        {
            if (item.IsEditable)
                item.IsInEditMode = EditMode;
        }

        #region Events

        public event EventHandler<FtpFileRenamedEventArgs> FtpFileRenamedEvent;

        #endregion Events

        #region Event-Handler

        /// <summary>
        /// Selected item within the tree changed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void treeFolderStructure_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
                SelectedFtpFilesystemFolder = e.NewValue as FtpFile;
        }

        /// <summary>
        /// KeyDown-Event within the tree view
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void treeFolderStructure_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource != null && e.OriginalSource is TreeViewItem)
            {
                var etb = MetroFtpClient.Infrastructure.Helper.VisualTreeHelpers.FindChild<EditableTextBlock>(e.OriginalSource as DependencyObject);

                if (etb != null)
                {
                    etb.TextChangedEvent += Etb_TextChangedEvent;

                    if (e.Key == Key.F2)
                        this.SetItemInEditMode(etb, true);
                }
            }
        }

        private void Etb_TextChangedEvent(object sender, Contracts.Events.TextChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                string newUrl = this.SelectedFtpFilesystemFolder.Url.AbsoluteUri.Replace(
                    this.SelectedFtpFilesystemFolder.Url.Segments.Last(), e.NewValue);

                var oldUrl = this.SelectedFtpFilesystemFolder.Url;

                this.SelectedFtpFilesystemFolder.Url = new Uri(newUrl);
                
                OnRaiseFtpFileRenamedEvent(new FtpFileRenamedEventArgs(e.OldValue, e.NewValue, oldUrl, this.SelectedFtpFilesystemFolder.Url));
            }

            ((EditableTextBlock)sender).TextChangedEvent -= Etb_TextChangedEvent;
        }

        /// <summary>
        /// MouseRightButtonDown-Event within the tree view
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void treeFolderStructure_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = MetroFtpClient.Infrastructure.Helper.VisualTreeHelpers.FindAncestor<TreeViewItem>(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                if (treeViewItem.DataContext != null && treeViewItem.DataContext is FtpFile)
                {
                    this.SelectedFtpFilesystemFolder = treeViewItem.DataContext as FtpFile;
                    treeViewItem.Focus();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Raise FTP-File renamed event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRaiseFtpFileRenamedEvent(FtpFileRenamedEventArgs e)
        {
            this.FtpFileRenamedEvent?.Invoke(this, e);
        }

        #endregion Event-Handler

        #region Dependency Properties

        public FtpFile FtpFilesystem
        {
            get { return (FtpFile)GetValue(FtpFilesystemProperty); }
            set { SetValue(FtpFilesystemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FtpFilesystem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FtpFilesystemProperty =
            DependencyProperty.Register("FtpFilesystem", typeof(FtpFile), typeof(FtpFilesystemControl), new PropertyMetadata(null));

        public FtpFile SelectedFtpFilesystemFolder
        {
            get { return (FtpFile)GetValue(SelectedFtpFilesystemFolderProperty); }
            set { SetValue(SelectedFtpFilesystemFolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedFtpFilesystemFolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedFtpFilesystemFolderProperty =
            DependencyProperty.Register("SelectedFtpFilesystemFolder", typeof(FtpFile), typeof(FtpFilesystemControl), new PropertyMetadata(null));

        public ICommand AddFtpFileToQueueCommand
        {
            get { return (ICommand)GetValue(AddFtpFileToQueueCommandProperty); }
            set { SetValue(AddFtpFileToQueueCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddFtpFileToQueueCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddFtpFileToQueueCommandProperty =
            DependencyProperty.Register("AddFtpFileToQueueCommand", typeof(ICommand), typeof(FtpFilesystemControl), new PropertyMetadata(null));

        public ICommand DeleteFtpFileCommand
        {
            get { return (ICommand)GetValue(DeleteFtpFileCommandProperty); }
            set { SetValue(DeleteFtpFileCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteFtpFileCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteFtpFileCommandProperty =
            DependencyProperty.Register("DeleteFtpFileCommand", typeof(ICommand), typeof(FtpFilesystemControl), new PropertyMetadata(null));

        #endregion Dependency Properties

    }
}