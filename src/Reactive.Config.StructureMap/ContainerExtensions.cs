using System;
using StructureMap;

namespace Reactive.Config.StructureMap
{
    public static class ContainerExtensions 
    {
        public static void ReactiveConfig(this ConfigurationExpression config, Action<IReactiveConfigRegistry> action)
        {
            config.For<IKeyPathProvider>().Use<NamespaceKeyPathProvider>();
            config.For<IConfigurationResultStore>().Singleton().Use<ConfigurationResultStore>();

            var configRegsitry = new ReactiveConfigRegsitry(config);

            action(configRegsitry);
        }
    }
}