namespace Reactive.Config
{
    public interface IKeyPathProvider
    {
        string GetKeyPath<T>(T value) where T : class, IConfigured, new();
    }
}
