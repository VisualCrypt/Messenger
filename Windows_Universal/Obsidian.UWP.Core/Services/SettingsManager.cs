using System;
using System.IO;
using System.Text;
using Windows.Storage;
using Obsidian.Applications.Models.Settings;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.Services.PortableImplementations;

namespace Obsidian.UWP.Core.Services
{
    public sealed class SettingsManager : AbstractSettingsManager
    {

        public SettingsManager(Container container):base(container)
        {
            
        }
        /// <summary>
        /// In this version of the App, the CurrentDirectoryName set from OpenFile is ignored.
        /// Instead, it will always be ApplicationData.Current.LocalFolder.Path.
        /// </summary>
        public override string CurrentDirectoryName
        {
            get
            {
                // in WPF: if (string.IsNullOrWhiteSpace(_currentDirectoryName) || !Directory.Exists(_currentDirectoryName))
                // here: always return the path of the local folder.
                return ApplicationData.Current.LocalFolder.Path;
            }
            set { }
        }

        public override void FactorySettings()
        {
            Log.Debug("Applying factory settings.");

            ChatSettings = new ChatSettings
            {
                RemoteDnsHostAddress = "visualcryptservice.cloudapp.net",
                RemoteUdpPort = 55555,
                RemoteTcpPort = 55556,
                Interval = 1000,
            };
            EditorSettings = new EditorSettings
            {
                IsWordWrapChecked = true,
                IsSpellCheckingChecked = false,
                PagePadding = 72,
                IsToolAreaVisible = false
            };
            CryptographySettings = new CryptographySettings { LogRounds = 10 };
            UpdateSettings = new UpdateSettings
            {
                Version = Aip.AssemblyVersion,
                SKU = Aip.AssemblyProduct,
                Date = DateTime.UtcNow,
                Notify = true
            };
        }


        protected override string ReadSettingsFile()
        {
            var settingsFilename = Path.Combine(ApplicationData.Current.LocalFolder.Path, SettingsFilename);
            if (File.Exists(settingsFilename))
                return File.ReadAllText(settingsFilename, Encoding.Unicode);
            return null;
        }

        protected override void WriteSettingsFile(string settingsFile)
        {
            File.WriteAllText(Path.Combine(ApplicationData.Current.LocalFolder.Path, SettingsFilename), settingsFile, Encoding.Unicode);
        }
    }
}
