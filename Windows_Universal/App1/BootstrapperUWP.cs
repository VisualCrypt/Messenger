using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Windows.UI.Xaml;
using Obsidian.Applications;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.Services.PortableImplementations;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.Api.Interfaces;
using Obsidian.Cryptography.NetStandard;
using Obsidian.UWP.Core.Data;
using Obsidian.UWP.Core.Network;
using Obsidian.UWP.Core.Services;
using Obsidian.UWP.Core.Storage;
using Obsidian.UWP.Services;

namespace Obsidian.UWP
{
    public static class BootstrapperUWP
    {
        static readonly Stopwatch StopWatch = new Stopwatch();

        public static void Run(App app)
        {
            VisualCryptApplication visualCryptApplication = null;
            try
            {
                StopWatch.Start();
                var container = CreateContainer();
                visualCryptApplication = new VisualCryptApplication(container);
                visualCryptApplication.Configure();
                app.Container = container;
            }
            catch (Exception e)
            {
                visualCryptApplication?.Container?.Get<ILog>()?.Exception(e);
            }
        }

        static Container CreateContainer()
        {
            var container = new Container("UWP Client");
            container.RegisterType<ILog, ReplayLogger>();
            container.Get<ILog>().Debug("Registering Services");
            container.RegisterObject<IAsyncRepository<Profile>>(new FStoreRepository<Profile>(FStoreInitializer.DefaultStore));
            container.RegisterObject<IAsyncRepository<Identity>>(new FStoreRepository<Identity>(FStoreInitializer.DefaultStore));
            container.RegisterObject<IAsyncRepository<Message>>(new FStoreRepository<Message>(FStoreInitializer.DefaultStore));
            container.RegisterType<IPhotoImportService, PhotoImportService>();
            BootstrapperCommon.RegisterPortableTypes(container);

            container.RegisterType<IDispatcher, CoreDispatcherService>();
            container.RegisterType<INavigationService, NavigationService>();

            container.RegisterType<IAssemblyInfoProvider, AssemblyInfoProvider>();
            container.Get<IAssemblyInfoProvider>().Assembly = typeof(BootstrapperUWP).GetTypeInfo().Assembly;

            container.RegisterType<IPlatform, Platform_NetStandard>();
            container.RegisterType<IVisualCrypt2Service, VisualCrypt2Service>();

            // objects - as we create them here, they will pull dependencies now :-|

            container.RegisterObject<AbstractSettingsManager>(new SettingsManager(container));
            container.RegisterObject<IMessageBoxService>(new MessageBoxService(container));

            container.RegisterObject<IFileService>(new FileService(container));
            container.RegisterObject<IUdpConnection>(new UdpConnection(container));
            container.RegisterObject<ITcpConnection>(new TcpConnection(container));

            BootstrapperCommon.RegisterPortableObjects(container);
         
            return container;
        }

        internal static void StopMeasureStartupTime(Container container)
        {
            container.Get<ILog>().Debug(string.Format(CultureInfo.InvariantCulture, "Loading completed after {0}ms.",
                   StopWatch.ElapsedMilliseconds));
            StopWatch.Stop();
        }

        public static Container GetContainer(this Application application)
        {
            return ((App)application).Container;
        }
    }
}