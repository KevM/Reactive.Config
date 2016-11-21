using System.Reactive.Linq;

namespace Reactive.Config
{
    public interface IConfigurationSource
    {
        bool Handles<T>() where T : class, IConfigured, new();
        ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new();
    }
}
