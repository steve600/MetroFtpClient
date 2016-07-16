using MetroFtpClient.Infrastructure.Constants;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MetroFtpClient.Core.Base
{
    public abstract class ViewModelPopupBase : ViewModelBase, IInteractionRequestAware
    {
        #region CTOR

        /// <summary>
        /// CTOR
        /// </summary>
        protected ViewModelPopupBase(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.InitializeCommands();
        }

        #endregion CTOR

        #region Commands

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            this.ClosePopupCommand = new DelegateCommand(this.ClosePopup, this.CanClosePopup);
        }

        public ICommand ClosePopupCommand { get; private set; }

        private bool CanClosePopup()
        {
            return true;
        }

        private void ClosePopup()
        {
            if (this.FinishInteraction != null)
                FinishInteraction();
        }

        #endregion Commands

        #region Properties

        #region IInteractionRequestAware

        private INotification notification = null;

        /// <summary>
        /// Notification will be set by the PopupWindowAction when the popup is shown.
        /// </summary>
        public INotification Notification
        {
            get
            {
                return this.notification;
            }
            set
            {
                this.SetProperty<INotification>(ref this.notification, value);
            }
        }

        /// <summary>
        /// FinishInteraction will be set by the PopupWindowAction when the popup is shown.
        /// </summary>
        public Action FinishInteraction { get; set; }

        #endregion IInteractionRequestAware

        /// <summary>
        /// Size to content
        /// </summary>
        public virtual SizeToContent PopupSizeToContent
        {
            get
            {
                return SizeToContent.WidthAndHeight;
            }
        }

        /// <summary>
        /// Resize mode
        /// </summary>
        public virtual ResizeMode PopupResizeMode
        {
            get
            {
                return ResizeMode.NoResize;
            }
        }

        #endregion Properties
    }
}