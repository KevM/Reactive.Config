using System;
using System.Threading;
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
                    s.AssemblyContainingType<JsonConfigurationSourceSettings>();
                    s.Convention<ReactiveConfig>();
                });
            });

            var jsonConfigurationSource = container.GetInstance<JsonConfigurationSource>();

            var original = new MyConfigured();
            jsonConfigurationSource.CreateConfigFile(original);
            Console.WriteLine(original);

            PrintContainerConfig(container, original);

            var updated = new MyConfigured
            {
                EnabledOn = DateTime.UtcNow.AddDays(7),
                IsEnabled = false,
                MyAppKey = "should be different"
            };

            jsonConfigurationSource.CreateConfigFile(updated);

            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                if (PrintContainerConfig(container, original))
                {
                    break;
                }
            }
            
            return 0;
        }

        private static bool PrintContainerConfig(IContainer container, MyConfigured original)
        {
            var fromContainer = container.GetInstance<MyConfigured>();

            var isDifferent = !fromContainer.Equals(original);

            if (isDifferent)
            {
                Console.WriteLine($"Config from the container is {(isDifferent ? "different" : "the same")}.");
                Console.WriteLine(fromContainer);
                return true;
            }

            return false;
        }
    }
}