using System;

namespace Reactive.Config
{
    public class ConfigurationResult<T>
        where T: class, IConfigured, new()
    {
        public IObservable<T> Observable { get; set; }
        public T Result { get; set; }

        public ConfigurationResult(T result, IObservable<T> observable)
        {
            Result = result;
            Observable = observable;
        }
    }
}