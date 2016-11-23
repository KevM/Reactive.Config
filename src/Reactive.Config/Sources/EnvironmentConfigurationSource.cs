using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Reactive.Config.Extensions;

namespace Reactive.Config.Sources
{
    public class EnvironmentConfigurationSource : IConfigurationSource
    {
        private readonly IKeyPathProvider _keyPathProvider;

        public EnvironmentConfigurationSource(IKeyPathProvider keyPathProvider)
        {
            _keyPathProvider = keyPathProvider;
        }

        public bool Handles<T>() where T : class, IConfigured, new()
        {
            return ConfiguredEnvironmentEntries<T>().Any();
        }

        public ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new()
        {
            var configured = new T();

            const BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
            var typePropertiesByName = typeof(T).GetProperties(flags).ToDictionary(k=>k.Name);
            var configPropertiesFound = ConfigByPropertyName<T>();

            foreach (var keypair in configPropertiesFound)
            {
                if (!typePropertiesByName.ContainsKey(keypair.Key)) continue;
                var propertyInfo = typePropertiesByName[keypair.Key];
                var newValue = propertyInfo.BindValue(keypair.Value);
                propertyInfo.SetValue(configured, newValue);
            }

            return new ConfigurationResult<T>(configured, Observable.Never<T>());
        }

        private IDictionary<string, string> ConfigByPropertyName<T>() where T : class, IConfigured, new()
        {
            return ConfiguredEnvironmentEntries<T>()
                .ToDictionary(k => GetKeyTail((string) k.Key), v => (string) v.Value);
        }

        private IEnumerable<DictionaryEntry> ConfiguredEnvironmentEntries<T>() where T : class, IConfigured, new()
        {
            var keyPrefix = _keyPathProvider.GetKeyPath<T>();

            return Environment.GetEnvironmentVariables()
                .OfType<DictionaryEntry>()
                .Where(entry => ((string) entry.Key).StartsWith(keyPrefix));
        }

        public static string GetKeyTail(string key)
        {
            var indexOfLastDot = key.LastIndexOf(".", StringComparison.InvariantCulture);

            return indexOfLastDot < 0 ? key : key.Substring(indexOfLastDot+1);
        }
    }
}