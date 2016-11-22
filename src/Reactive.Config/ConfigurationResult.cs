using System;

namespace Reactive.Config
{
    public class ConfigurationResult<T>
        where T: class, IConfigured, new()
    {
        public IObservable<T> Observable { get; set; }
        public T Result { get; set; }

        // TODO add resolution about which properties or maybe keys were updated?
        // This would be good for diagnostics and maybe it is a bad idea to have 
        // each source doing some much reflection. Maybe just do it one time then the sources
        // can consume simple keypath + keys metadata.

        //public PropertyInfo[] PropertyInfos { get; set; }        

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