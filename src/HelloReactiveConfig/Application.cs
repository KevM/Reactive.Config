using System;
using Reactive.Config;
using Reactive.Config.Sources;
using Reactive.Config.StructureMap;
using StructureMap;
using StructureMap.Graph;

namespace HelloReactiveConfig
{
    public static class Application
    {
        public static int Main(string[] args)
        {
            var container = new Container(_ =>
            {
                _.ReactiveConfig(rc =>
                {
                    rc.AddSource<IdentityConfigurationSource>();
                });

                _.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.Convention<ReactiveConfig>();
                });
            });

            var configured = container.GetInstance<MyConfigured>();

            Console.WriteLine(configured.IsEnabled);
            Console.WriteLine(configured.EnabledOn);
            Console.WriteLine(configured.MyAppKey);

            return 0;
        }
    }
}