using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Reactive.Config
{
    public interface IConfigured {}

    public interface IKeySpaceProvider
    {
        string GetKeySpace<T>(T value) where T : struct, IConfigured;
    }

    public interface IConfigurationSource
    {
        bool Handles(string keySpace);
        IObservable<T> Get<T>(T value, string keySpace) where T : struct, IConfigured;
    }

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

    public interface IConfigurationRegistry
    {
        IConfigurationProvider GetProvider();
        void AddSource(IConfigurationSource source);
    }

    public class ConfigurationRegistry : IConfigurationRegistry
    {
        private IList<IConfigurationSource> _sources = new List<IConfigurationSource>();

        public void AddSource(IConfigurationSource source)
        {
            _sources.Add(source);
        }

        public void Add<T>() where T : struct, IConfigured
        {
            
        }

        public IConfigurationProvider GetProvider()
        {
            throw new System.NotImplementedException();
        }
    }
}
