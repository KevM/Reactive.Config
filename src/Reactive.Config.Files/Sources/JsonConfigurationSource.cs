using System;
using System.IO;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Reactive.Config.Files.Watcher;

namespace Reactive.Config.Files.Sources
{
    public class JsonConfigurationSource : IConfigurationSource
    {
        private readonly IKeyPathProvider _keyPathProvider;
        private readonly JsonConfigurationSourceSettings _settings;

        public JsonConfigurationSource(IKeyPathProvider keyPathProvider, JsonConfigurationSourceSettings settings)
        {
            _keyPathProvider = keyPathProvider;
            _settings = settings;
        }

        public bool Handles<T>() where T : class, IConfigured, new()
        {
            var settingsFile = GetSettingsFileInfo<T>();

            return settingsFile.Exists;
        }

        public ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new()
        {
            var pollingInterval = TimeSpan.FromSeconds(_settings.PollingIntervalInSeconds);
            var settingsFileInfo = GetSettingsFileInfo<T>();

            var model = GetSettingModel<T>(settingsFileInfo.FullName);

            var observable = settingsFileInfo
                .PollFile(pollingInterval)
                .Select(ConfigFileUpdated<T>);

            return new ConfigurationResult<T>(model, observable);
        }

        private static T ConfigFileUpdated<T>(Notification notification) where T : class, IConfigured, new()
        {
            switch (notification.NotificationType)
            {
                case Notification.Type.Changed:
                case Notification.Type.Created:
                    return GetSettingModel<T>(notification.FullName);
                default:
                    return new T();
            }
        }

        // Exposed for testing. Not on the interface.

        public FileInfo GetSettingsFileInfo<T>(string suffix = ".json") where T : class, IConfigured, new()
        {
            var settingsFileName = _keyPathProvider.GetKeyPath<T>() + suffix;
            var settingsFilePath = Path.Combine(_settings.SettingsFilePath, settingsFileName);
            return new FileInfo(settingsFilePath);
        }

        public FileInfo CreateConfigFile<T>(T model) where T : class, IConfigured, new()
        {
            var settingsFile = GetSettingsFileInfo<T>();

            if (settingsFile.Directory == null) throw new ArgumentException("File must be in a directory");
            if (!settingsFile.Directory.Exists)
            {
                settingsFile.Directory.Create();
            }

            using (var f = File.CreateText(settingsFile.FullName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(f, model);
            }

            return settingsFile;
        }

        private static T GetSettingModel<T>(string filePath) where T : class, IConfigured, new()
        {
            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                return (T) serializer.Deserialize(file, typeof(T));
            }
        }
    }
}