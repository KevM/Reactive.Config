namespace Reactive.Config
{
    /// <summary>
    /// Map configured type to a keyspace. This is used to namespace the configured type into a name value collection.
    /// 
    /// Note: This is pretty primative and likely needs to be pushed farther.
    /// </summary>
    public interface IKeyPathProvider
    {
        string GetKeyPath<T>() where T : class, IConfigured, new();
    }

    public class NamespaceKeyPathProvider: IKeyPathProvider
    {
        public string GetKeyPath<T>() where T : class, IConfigured, new()
        {
            var type = typeof(T);
            
            return type.FullName;
        }
    }
}
