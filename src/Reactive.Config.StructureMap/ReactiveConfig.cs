using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Reactive.Config.StructureMap
{
    public class ReactiveConfig : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (type.IsAbstract || !typeof(IConfigured).IsAssignableFrom(type)) return;

            registry
                .For(type)
                .Use("Reactive.Config", ctx =>
                {
                    var provider = ctx.GetInstance<IConfigurationProvider>();

                    var method = provider.GetType().GetMethod("Get");
                    var generic = method.MakeGenericMethod(ctx.RootType);
                    return generic.Invoke(this, null);
                });
        }
    }
}