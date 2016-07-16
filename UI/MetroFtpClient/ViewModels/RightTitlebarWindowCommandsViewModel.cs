using MetroFtpClient.Core.Base;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;

namespace MetroFtpClient.ViewModels
{
    public class RightTitlebarWindowCommandsViewModel : ViewModelBase
    {
        public RightTitlebarWindowCommandsViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
        }
    }
}