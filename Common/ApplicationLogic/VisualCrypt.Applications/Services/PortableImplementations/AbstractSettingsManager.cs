using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Applications.Services.PortableImplementations
{
    public abstract class AbstractSettingsManager
    {
        protected const string SettingsFilename = "VisualCryptSettings.txt";
        protected readonly ILog Log;
        protected readonly IAssemblyInfoProvider Aip;
        protected readonly Container Container;
        protected string _currentDirectoryName;

        protected AbstractSettingsManager(Container container)
        {
            Log = container.Get<ILog>();
            Aip = container.Get<IAssemblyInfoProvider>();
            Container = container;
            InitForBinding();
        }

        public ChatSettings ChatSettings { get; protected set; }
        public EditorSettings EditorSettings { get; protected set; }
        public CryptographySettings CryptographySettings { get; protected set; }
        public UpdateSettings UpdateSettings { get; protected set; }

        public abstract string CurrentDirectoryName { get; set; }




        public abstract void FactorySettings();

        protected abstract string ReadSettingsFile();

        protected abstract void WriteSettingsFile(string settingsFile);

        void InitForBinding()
        {
            var success = LoadSettings();
            if (!success || IsMigrationNesessary())
            {
                FactorySettings();
                SaveSettings();
            }
            ChatSettings.PropertyChanged += (s, e) => SaveSettings();
            EditorSettings.PropertyChanged += (s, e) => SaveSettings();
            CryptographySettings.PropertyChanged += (s, e) => SaveSettings();
            UpdateSettings.PropertyChanged += (s, e) => SaveSettings();
        }

        bool IsMigrationNesessary()
        {
            return (ChatSettings == null
                    || EditorSettings == null
                    || CryptographySettings == null
                    || UpdateSettings == null);
        }

        void SaveSettings()
        {
            try
            {
                if (EditorSettings == null)
                    return;

                // Collect the Settings
                var settings = new VisualCryptSettings
                {
                    ChatSettings = ChatSettings,
                    EditorSettings = EditorSettings,
                    CryptographySettings = CryptographySettings,
                    UpdateSettings = UpdateSettings
                };

                // Serialize, save
                var serializedSettings = Serialize(settings);
                WriteSettingsFile(serializedSettings);

                Log.Debug("Settings saved!");
            }
            catch (Exception e)
            {
                Log.Debug($"Could not save settings: {e.Message}");
            }
        }

        bool LoadSettings()
        {
            try
            {
                // Load, deserialize
                string serializedSettings = ReadSettingsFile();
                if (string.IsNullOrWhiteSpace(serializedSettings))
                    return false;
                var settings = Deserialize(serializedSettings);

                // Distribute
                ChatSettings = settings.ChatSettings;
               
                if (string.IsNullOrWhiteSpace(ChatSettings?.RemoteDnsHostAddress))
                    return false;
                EditorSettings = settings.EditorSettings;
                CryptographySettings = settings.CryptographySettings;
                UpdateSettings = settings.UpdateSettings;
                if (EditorSettings == null || CryptographySettings == null || 
                    UpdateSettings == null)
                    return false;
                Log.Debug("Settings loaded.");
                return true;
            }
            catch (Exception e)
            {
                Log.Debug($"Could not load settings: {e.Message}");
                return false;
            }
        }

        string Serialize(VisualCryptSettings settings)
        {
            using (var stream = new MemoryStream())
            {
                var ser = new DataContractSerializer(typeof(VisualCryptSettings));
                ser.WriteObject(stream, settings);
                var data = stream.ToArray();
                var serialized = Encoding.UTF8.GetString(data, 0, data.Length);
                return serialized;
            }
        }

        VisualCryptSettings Deserialize(string data)
        {

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var ser = new DataContractSerializer(typeof(VisualCryptSettings));
                var settings = (VisualCryptSettings)ser.ReadObject(stream);
                return settings;
            }
        }
    }
}





