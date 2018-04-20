using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Reactive.Config
{
    /// <summary>
    /// Aggregates all injected <see cref="IConfigurationSource"/> types to produce a result. 
    /// Starting with a new <see cref="IConfigured"/> type, each source handling the type, enriches the result.
    /// </summary>
    public interface IConfigurationSourceResolver
    {
        /// <summary>
        /// Construct a new configured type (T) from one or more <see cref="IConfigurationSource">configuration sources</see>.
        /// </summary>
        /// <typeparam name="T">Configured type to create.</typeparam>
        /// <returns>Configuration result for the requested configured type.</returns>
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

        /// <summary>
        /// Enrich the given <see cref="accumulator">accmulated result</see> with the given <see cref="source"/>. 
        /// The given result observable is merged with the new result's observable. 
        /// This chains them together so that if in the future any source observes a change to the configured type it is propagated to the <see cref="IConfigurationResultStore"/>.
        /// </summary>
        /// <typeparam name="T">Configured type</typeparam>
        /// <param name="accumulator">Results built potentially from previously visited <see cref="IConfigurationSource">configuration sources</see>.</param>
        /// <param name="source">The current source being used to enrich the configuration result.</param>
        /// <returns>The enriched configuration result.</returns>
        public static ConfigurationResult<T> MergeSourceResults<T>(ConfigurationResult<T> accumulator, IConfigurationSource source)
            where T : class, IConfigured, new()
        {
            var nextResult = source.Get(accumulator);
            nextResult.Observable = accumulator.Observable.Merge(nextResult.Observable);
            return nextResult;
        }
    }
}
