using System;
using Reactive.Config.Files.Sources;
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
                    rc.AddSource<JsonConfigurationSource>();
                });

                _.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.Convention<ReactiveConfig>();
                });
            });

            var test = new MyConfigured();
            container.GetInstance<JsonConfigurationSource>().CreateConfigFile(test);

            var configured = container.GetInstance<MyConfigured>();

            Console.WriteLine(configured.IsEnabled);
            Console.WriteLine(configured.EnabledOn);
            Console.WriteLine(configured.MyAppKey);

            return 0;
        }
    }
}