using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace Reactive.Config.Sources
{
    /// <summary>
    /// Simple environment configuration source which looks for and binds environment values for keys found
    /// with names matching the namespace, type name, and property name of the <see cref="IConfigured">configured type</see>. 
    /// For example an environment value with the key "Reactive.Config.Tests.TestConfigured.IsEnabled" will be bound to the cooresponding property.
    /// </summary>
    public class EnvironmentConfigurationSource : IConfigurationSource
    {
        private readonly IKeyPathProvider _keyPathProvider;
        private readonly IPropertyValueBinder _propertyValueBinder;

        public EnvironmentConfigurationSource(IKeyPathProvider keyPathProvider, IPropertyValueBinder propertyValueBinder)
        {
            _keyPathProvider = keyPathProvider;
            _propertyValueBinder = propertyValueBinder;
        }

        public bool Handles<T>() where T : class, IConfigured, new()
        {
            return ConfiguredEnvironmentEntries<T>().Any();
        }

        public ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new()
        {
            // TODO move all this custom rolled model binding to something more robust.
            // https://lostechies.com/chadmyers/2011/06/08/cool-stuff-in-fubucore-no-7-model-binding/ <- this is what I've used in the past but it is pretty old.

            var configured = new T();

            const BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
            var typePropertiesByName = typeof(T).GetProperties(flags).ToDictionary(k=>k.Name);
            var configPropertiesFound = ConfigByPropertyName<T>();

            foreach (var keypair in configPropertiesFound)
            {
                if (!typePropertiesByName.ContainsKey(keypair.Key)) continue;
                var propertyInfo = typePropertiesByName[keypair.Key];
                var newValue = _propertyValueBinder.BindValue(propertyInfo, keypair.Value);
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

            var entries = Environment.GetEnvironmentVariables()
                .OfType<DictionaryEntry>()
                .Where(entry => ((string) entry.Key).StartsWith(keyPrefix));

            return entries;
        }

        public static string GetKeyTail(string key)
        {
            var indexOfLastDot = key.LastIndexOf(".", StringComparison.InvariantCulture);

            return indexOfLastDot < 0 ? key : key.Substring(indexOfLastDot+1);
        }
    }
}
