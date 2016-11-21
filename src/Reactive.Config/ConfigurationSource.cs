using System;

namespace Reactive.Config
{
    public interface IConfigurationSource
    {
        bool Handles<T>() where T : struct, IConfigured;
        IObservable<T> Get<T>(T value) where T : struct, IConfigured;
    }
}
