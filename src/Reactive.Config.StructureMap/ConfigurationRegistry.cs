using System.Collections.Generic;
using StructureMap;

namespace Reactive.Config.StructureMap
{
    public interface IReactiveConfigRegistry
    {
        void AddSource<T>() where T : IConfigurationSource;
    }

    public class ReactiveConfigRegsitry : IReactiveConfigRegistry
    {
        private readonly ConfigurationExpression _config;

        public ReactiveConfigRegsitry(ConfigurationExpression config)
        {
            _config = config;
        }

        public void AddSource<T>() where T : IConfigurationSource
        {
            _config.For<IConfigurationSource>().Add<T>();
        }
    }
}
