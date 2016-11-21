using System.Collections.Generic;

namespace Reactive.Config
{
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

        public void Add<T>() where T : class, IConfigured, new()
        {

        }

        public IConfigurationProvider GetProvider()
        {
            throw new System.NotImplementedException();
        }
    }
}
