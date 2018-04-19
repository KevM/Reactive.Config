using System;
using System.Linq;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Reactive.Config.StructureMap
{
    public class ReactiveConfig : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            var configuredMarkerType = typeof(IConfigured);
            var configuredTypes = types
                .FindTypes(TypeClassification.Concretes)
                .Where(t => configuredMarkerType.IsAssignableFrom(t));

            foreach (var type in configuredTypes)
            {
                registry
                    .For(type)
                    .Use("Reactive.Config", ctx =>
                    {
                        var provider = ctx.GetInstance<IConfigurationProvider>();

                        var method = provider.GetType().GetMethod("Get");
                        if (method == null)
                        {
                            throw new NotImplementedException("IConfigurationProvider has to have a GET method. It is in the interface!");
                        }

                        var generic = method.MakeGenericMethod(type);
                        return generic.Invoke(provider, null);
                    });
            }
        }
    }
}
