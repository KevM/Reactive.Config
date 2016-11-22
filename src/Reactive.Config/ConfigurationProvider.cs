namespace Reactive.Config
{
    public interface IConfigurationProvider
    {
        T Get<T>() where T : class, IConfigured, new();
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly IConfigurationResultStore _resultStore;
        private readonly IConfigurationSourceResolver _sourceResolver;

        public ConfigurationProvider(IConfigurationResultStore resultStore, IConfigurationSourceResolver sourceResolver)
        {
            _resultStore = resultStore;
            _sourceResolver = sourceResolver;
        }

        public T Get<T>() where T : class, IConfigured, new()
        {
            var existingResult = _resultStore.Get<T>();

            if (existingResult != null) return existingResult;

            var configurationResult = _sourceResolver.Resolve<T>();

            _resultStore.Store(configurationResult);

            return configurationResult.Result;
        }
    }
}
