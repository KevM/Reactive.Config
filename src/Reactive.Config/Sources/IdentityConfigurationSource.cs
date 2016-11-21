using System.Reactive.Linq;

namespace Reactive.Config.Sources
{
    public class IdentityConfigurationSource : IConfigurationSource
    {
        public bool ShouldHandle { get; set; } = true;

        public bool Handles<T>() where T : class, IConfigured, new()
        {
            return ShouldHandle;
        }

        public ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new()
        {
            var resultOut = new T();
            var observable = Observable.Never<T>();

            return new ConfigurationResult<T>(resultOut, observable);
        }
    }
}