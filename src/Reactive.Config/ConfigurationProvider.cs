using System;
using System.Collections.Generic;
using System.Linq;

namespace Reactive.Config
{
    public interface IConfigurationProvider
    {
        T Get<T>() where T : struct, IConfigured;
//        IObservable<T> GetObservable<T>() where T : struct, IConfigured;
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly IEnumerable<IConfigurationSource> _sources;

        public ConfigurationProvider(IEnumerable<IConfigurationSource> sources)
        {
            _sources = sources;
        }

        public T Get<T>() where T : struct, IConfigured
        {
            var handlingSources = _sources.Where(s => s.Handles<T>());

            return new T();
//            var acc = new T();
//            return _sources.Aggregate(acc, (a, source) =>{
//                if()
//            });
        }

//        public IObservable<T> GetObservable<T>() where T : struct, IConfigured
//        {
//            throw new NotImplementedException();
//        }
    }
}
