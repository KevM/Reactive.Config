using System.Collections.Generic;
using System.Linq;

namespace Reactive.Config
{
    public interface IConfigurationProvider
    {
        T Get<T>() where T : class, IConfigured, new();
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly IEnumerable<IConfigurationSource> _sources;
        private readonly IConfigurationResultStore _resultStore;

        public ConfigurationProvider(IEnumerable<IConfigurationSource> sources, IConfigurationResultStore resultStore)
        {
            _sources = sources;
            _resultStore = resultStore;
        }

        public T Get<T>() where T : class, IConfigured, new()
        {
            var existingResult = _resultStore.Get<T>();

            if (existingResult != null) return existingResult;

            var handlingSources = _sources.Where(s => s.Handles<T>()).ToList();

            var acc = new ConfigurationResult<T>(new T());

            var configurationResult = handlingSources.Aggregate(acc, (a, s) => s.Get(a));

            _resultStore.Store(configurationResult);

            return configurationResult.Result;
        }
    }
}
