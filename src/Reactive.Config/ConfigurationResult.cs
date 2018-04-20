using System;

namespace Reactive.Config
{
    // TODO should this be a struct as we are enriching the result at each level.

    /// <summary>
    /// The product of a <see cref="IConfigurationSource"/>. 
    /// This is aggregated by the <see cref="IConfigurationSourceResolver"/> 
    /// and stored by the <see cref="IConfigurationResultStore"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigurationResult<T>
        where T: class, IConfigured, new()
    {
        /// <summary>
        /// Should the configuration source(s), which created this result, "notice" any 
        /// changes to the state of this <see cref="IConfigured">configured type</see>. 
        /// This observable will receive the updated state of the configured type.
        /// </summary>
        public IObservable<T> Observable { get; set; }

        /// <summary>
        /// The current state of the configured type. 
        /// </summary>
        public T Result { get; set; }

        public ConfigurationResult(T result, IObservable<T> observable)
        {
            Result = result;
            Observable = observable;
        }

        public static ConfigurationResult<T> Create(T result = null, IObservable<T> observable = null)
        {
            result = result ?? new T();
            observable = observable ?? System.Reactive.Linq.Observable.Empty<T>();

            return new ConfigurationResult<T>(result, observable);
        }
    }
}
