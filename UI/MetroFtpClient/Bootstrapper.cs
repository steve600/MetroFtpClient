using Dragablz;
using MetroFtpClient.Events;
using MetroFtpClient.Ftp.Contracts.Interfaces;
using MetroFtpClient.Ftp.FtpClient;
using MetroFtpClient.Infrastructure;
using MetroFtpClient.Infrastructure.Constants;
using MetroFtpClient.Infrastructure.Events;
using MetroFtpClient.Infrastructure.Interfaces;
using MetroFtpClient.Infrastructure.Services;
using MetroFtpClient.RegionAdapter;
using MetroFtpClient.Services;
using MetroFtpClient.Views;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MetroFtpClient
{
    internal class Bootstrapper : UnityBootstrapper
    {
        private AutoResetEvent WaitForCreation { get; set; }

        /// <summary>
        /// The shell object
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject CreateShell()
        {
            Container.RegisterInstance(typeof(Window), WindowNames.MainWindowName, Container.Resolve<MainWindow>(), new ContainerControlledLifetimeManager());
            return Container.Resolve<Window>(WindowNames.MainWindowName);

            //return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// Initialize shell (MainWindow)
        /// </summary>
        protected override void InitializeShell()
        {
            // Register views
            var regionManager = this.Container.Resolve<IRegionManager>();
            if (regionManager != null)
            {
                regionManager.RegisterViewWithRegion(RegionNames.LeftWindowCommandsRegion, typeof(Views.LeftTitlebarWindowCommands));
                regionManager.RegisterViewWithRegion(RegionNames.RightWindowCommandsRegion, typeof(Views.RightTitlebarWindowCommands));
                regionManager.RegisterViewWithRegion(RegionNames.FlyoutRegion, typeof(Views.ApplicationSettings));
            }

            // Navigate to introduction page
            regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.OverviewPage);
        }

        /// <summary>
        /// Configure the DI-Container
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Register views for navigation
            Prism.Unity.UnityExtensions.RegisterTypeForNavigation<Views.SystemInfo>(Container, ViewNames.SystemInformationView);

            //Prism.Unity.UnityExtensions.RegisterTypeForNavigation<object, >(Container, ViewNames.OverviewPage);

            Container.RegisterType<object, Views.OverviewPage>(ViewNames.OverviewPage);

            //Container.RegisterInstance(typeof(object), ViewNames.OverviewPage, new Model.TabContent(ViewNames.OverviewPage, new Views.IntroductionPage()));

            // Application commands
            Container.RegisterType<IApplicationCommands, ApplicationCommandsProxy>();

            // Flyout service
            Container.RegisterInstance<IFlyoutService>(Container.Resolve<FlyoutService>());

            // Localizer service
            Container.RegisterInstance(typeof(ILocalizerService),
                ServiceNames.LocalizerService,
                new LocalizerService("de-DE"),
                new Microsoft.Practices.Unity.ContainerControlledLifetimeManager());

            Container.RegisterInstance<IConnectionManager>(GlobalConstants.ConnectionManager, Container.Resolve<ConnectionManager>(), new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Configure region adapter mappings
        /// </summary>
        /// <returns></returns>
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();
            var regionBehaviorFactory = Container.Resolve<IRegionBehaviorFactory>();
            mappings.RegisterMapping(typeof(TabablzControl), new TabablzControlRegionAdapter(regionBehaviorFactory));
            return mappings;
        }

        /// <summary>
        /// Initialize application modules
        /// </summary>
        protected override void InitializeModules()
        {
            base.InitializeModules();

            // Show SplashScreen
            this.ShowSplashScreen();

            IModule prismModule = null;

            // FTP-Module
            Container.Resolve<IEventAggregator>().GetEvent<SplashScreenStatusMessageUpdateEvent>().Publish("FTP module ...");
            prismModule = Container.Resolve<MetroFtpClient.Ftp.FtpModule>();
            prismModule.Initialize();

            System.Threading.Thread.Sleep(2000);

            // Set StatusBarMessage
            var statusBarMessage = Container.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("MetroFtpClient:Resources:ApplicationReadyStatusBarMessage");
            Container.Resolve<IEventAggregator>().GetEvent<UpdateStatusBarMessageEvent>().Publish(statusBarMessage);

            // Message display service
            Container.RegisterInstance<IMetroMessageDisplayService>(ServiceNames.MetroMessageDisplayService, Container.Resolve<MetroMessageDisplayService>(), new ContainerControlledLifetimeManager());

            // Show MainWindow
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Show SplashScreen
        /// </summary>
        private void ShowSplashScreen()
        {
            //Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
            //{
            //    Container.Resolve<IEventAggregator>().GetEvent<CloseSplashScreenEvent>().Publish(new CloseSplashScreenEvent());
            //}));
            
            //Application.Current.Dispatcher.BeginInvoke(
            //   (Action)(() =>
            //   {
            //         // Register SplashScreen
            //         //Container.RegisterInstance<Views.SplashScreen>("SplashScreen", new Views.SplashScreen(), new ContainerControlledLifetimeManager());
            //         var splash = Container.Resolve<Views.SplashScreen>("SplashScreen");

            //         // Subscribe for Closing-Event
            //         Container.Resolve<IEventAggregator>().GetEvent<CloseSplashScreenEvent>().Subscribe(e => splash.Dispatcher.BeginInvoke((Action)(() => { splash.DataContext = null; splash.Dispatcher.InvokeShutdown(); splash.Close(); })), ThreadOption.PublisherThread, true);

            //       splash.Show();
            //   }));
        }
    }
}