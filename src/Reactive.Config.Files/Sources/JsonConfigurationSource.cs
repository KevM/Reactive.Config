using System;
using System.IO;
using System.Reactive.Linq;
using Newtonsoft.Json;

namespace Reactive.Config.Files.Sources
{
    public class JsonConfigurationSourceSettings : IConfigured
    {
        public string SettingsFilePath { get; set; } = Environment.CurrentDirectory;
    }

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

        public FileInfo GetSettingsFileInfo<T>(string suffix = ".json") where T : class, IConfigured, new()
        {
            var settingsFileName = _keyPathProvider.GetKeyPath<T>() + suffix;
            var settingsFilePath = Path.Combine(_settings.SettingsFilePath, settingsFileName);
            return new FileInfo(settingsFilePath);
        }

        public ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new()
        {
            var model = GetSettingModel<T>();
            var observable = Observable.Never<T>();

            return new ConfigurationResult<T>(model, observable);
        }

        private T GetSettingModel<T>() where T : class, IConfigured, new()
        {
            using (var file = File.OpenText(GetSettingsFileInfo<T>().FullName))
            {
                var serializer = new JsonSerializer();
                return (T) serializer.Deserialize(file, typeof(T));
            }
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
    }
}