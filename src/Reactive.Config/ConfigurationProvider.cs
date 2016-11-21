using System;
using System.Collections.Generic;

namespace Reactive.Config
{
    public interface IConfigurationProvider
    {
        T Get<T>() where T : struct, IConfigured;
        IObservable<T> GetObservable<T>() where T : struct, IConfigured;
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private IList<IConfigurationSource> _sources = new List<IConfigurationSource>();

        public T Get<T>() where T : struct, IConfigured
        {
            // get T from dictionary if there 
            // if not there build one from
            throw new NotImplementedException();
        }

        public IObservable<T> GetObservable<T>() where T : struct, IConfigured
        {
            throw new NotImplementedException();
        }
    }
}
