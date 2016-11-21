using System;

namespace Reactive.Config
{
    public interface IConfigurationSource
    {
        bool Handles<T>(string keySpace = null) where T : struct, IConfigured;
        IObservable<T> Get<T>(T value, string keySpace = null) where T : struct, IConfigured;
    }
}
