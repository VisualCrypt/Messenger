using System;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.Workers;
using Obsidian.Cryptography.Api.Interfaces;

namespace Obsidian.Applications
{
    public class VisualCryptApplication : IDisposable
    {
        readonly IFileService _fileService;
        readonly ILog _log;
        readonly AppState _appState;

        public VisualCryptApplication(Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            Container = container;
            _log = Container.Get<ILog>();
            _fileService = container.Get<IFileService>();
            _appState = container.Get<AppState>();
        }

        public Container Container { get; }

        public void Configure(string localFolderPathOverride = null)
        {
            if (localFolderPathOverride != null)
                Container.Get<IFileService>().SetLocalFolderPathForTests(localFolderPathOverride);
            var localFolderPath = _fileService.GetLocalFolderPath();
            _log.Debug($"InstallLocation: {_fileService.GetInstallLocation()}");
            _log.Debug($"LocalFolder: {localFolderPath}");

            Container.Get<IVisualCrypt2Service>().Init(Container.Get<IPlatform>(),  Container.Name);
        }

        public void Dispose()
        {
            Container.Get<ChatWorker>().StopRunLoopAndDisconnectAll();
            Container.Dispose();
        }


    }
}
