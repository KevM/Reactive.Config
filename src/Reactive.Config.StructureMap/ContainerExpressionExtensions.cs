using System;
using StructureMap;

namespace Reactive.Config.StructureMap
{
    public static class ContainerExpressionExtensions 
    {
        public static void ReactiveConfig(this ConfigurationExpression config, Action<IReactiveConfigRegistry> action)
        {
            config.Scan(s =>
            {
                s.AssemblyContainingType<IConfigured>();
                s.Convention<ReactiveConfig>();
                s.WithDefaultConventions();
            });

            config.For<IConfigurationResultStore>().Singleton().Use<ConfigurationResultStore>();
            config.For<IKeyPathProvider>().Use<NamespaceKeyPathProvider>();

            var configRegsitry = new ReactiveConfigRegsitry(config);

            action(configRegsitry);
        }
    }
}