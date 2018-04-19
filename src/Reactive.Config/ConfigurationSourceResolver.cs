using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Reactive.Config
{
    public interface IConfigurationSourceResolver
    {
        ConfigurationResult<T> Resolve<T>() where T : class, IConfigured, new();
    }

    public class ConfigurationSourceResolver : IConfigurationSourceResolver
    {
        private readonly IEnumerable<IConfigurationSource> _sources;

        public ConfigurationSourceResolver(IEnumerable<IConfigurationSource> sources)
        {
            _sources = sources;
        }
        
        public ConfigurationResult<T> Resolve<T>() where T : class, IConfigured, new()
        {
            var handlingSources = _sources.Where(s => s.Handles<T>()).ToList();

            var accumulator = new ConfigurationResult<T>(new T(), Observable.Empty<T>());

            var configurationResult = handlingSources.Aggregate(accumulator, MergeSourceResults);
            return configurationResult;
        }

        public static ConfigurationResult<T> MergeSourceResults<T>(ConfigurationResult<T> accumulator, IConfigurationSource source)
            where T : class, IConfigured, new()
        {
            var nextResult = source.Get(accumulator);
            nextResult.Observable = accumulator.Observable.Merge(nextResult.Observable);
            return nextResult;
        }
    }
}
