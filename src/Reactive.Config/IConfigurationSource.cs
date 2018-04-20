namespace Reactive.Config
{
    /// <summary>
    /// Clients should not use these sources directly but rather should aggregate them through the <see cref="IConfigurationProvider"/>.
    /// </summary>
    public interface IConfigurationSource
    {
        /// <summary>
        /// Does this configuraiton source support the requested <see cref="IConfigured"/> type?
        /// </summary>
        /// <typeparam name="T">Configured type being requested.</typeparam>
        /// <returns>True if this instance of a configuration source supports the requested type.</returns>
        bool Handles<T>() where T : class, IConfigured, new();

        /// <summary>
        /// Enrich the given <see cref="result"/> input result with what this configuration source can add. 
        /// Note: This source may not be the only one populating this <see cref="IConfigured"/> type.
        /// </summary>
        /// <typeparam name="T">Configured type being requested.</typeparam>
        /// <param name="result">We are not necessarily the first source in use. We enrich this given result.</param>
        /// <returns>In an attempt to look functional we return the given result after we've enriched it.</returns>
        ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new();
    }
}
