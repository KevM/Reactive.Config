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
            var settingsFilePath = GetSettingsFilePath<T>();

            return File.Exists(settingsFilePath);
        }

        private string GetSettingsFilePath<T>() where T : class, IConfigured, new()
        {
            var settingsFileName = _keyPathProvider.GetKeyPath<T>();
            var settingsFilePath = Path.Combine(_settings.SettingsFilePath, settingsFileName);
            return settingsFilePath;
        }

        public ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new()
        {
            var model = GetSettingModel<T>();

            return new ConfigurationResult<T>(model, Observable.Never<T>());
        }

        private T GetSettingModel<T>() where T : class, IConfigured, new()
        {
            using (var file = File.OpenText(GetSettingsFilePath<T>()))
            {
                var serializer = new JsonSerializer();
                return (T) serializer.Deserialize(file, typeof(T));
            }
        }

        public FileInfo CreateConfigFile<T>(T model) where T : class, IConfigured, new()
        {
            var file = new FileInfo(GetSettingsFilePath<T>());

            if (file.Directory == null) throw new ArgumentException("File must be in a directory");
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            using (var f = File.CreateText(file.FullName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(f, model);
            }

            return file;
        }
    }
}