using System;
using Reactive.Config.Sources;
using Reactive.Config.StructureMap;
using StructureMap;

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
            });

            var configured = container.GetInstance<MyConfigured>();

            Console.WriteLine(configured.IsEnabled);
            Console.WriteLine(configured.EnabledOn);
            Console.WriteLine(configured.MyAppKey);

            return 0;
        }
    }
}