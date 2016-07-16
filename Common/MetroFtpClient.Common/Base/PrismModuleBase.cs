using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace MetroFtpClient.Core.Base
{
    public class PrismModuleBase : IModule
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="unityContainer">The Unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        public PrismModuleBase(IUnityContainer unityContainer, IRegionManager regionManager)
        {
            this.UnityContainer = unityContainer;
            this.RegionManager = regionManager;

            // Add resource dictionaries
            this.AddResourceDictionaries();
        }

        #endregion Ctor

        #region Interface IModule

        /// <summary>
        /// Initialize Module
        /// </summary>
        public virtual void Initialize()
        {
        }

        #endregion Interface IModule

        /// <summary>
        /// Add Resource-Dictionaries
        /// </summary>
        public virtual void AddResourceDictionaries()
        {
        }

        #region Properties

        private IUnityContainer unityContainer;

        /// <summary>
        /// The Unity container
        /// </summary>
        public IUnityContainer UnityContainer
        {
            get { return this.unityContainer; }
            private set { this.unityContainer = value; }
        }

        private IRegionManager regionManager;

        /// <summary>
        /// The region manager
        /// </summary>
        public IRegionManager RegionManager
        {
            get { return this.regionManager; }
            private set { this.regionManager = value; }
        }

        #endregion Properties
    }
}