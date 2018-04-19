using System;
using System.Collections.Generic;
using System.Threading;

namespace Reactive.Config
{
    public interface IConfigurationResultStore
    {
        void Store<T>(ConfigurationResult<T> configurationResult) where T : class, IConfigured, new();
        T Get<T>() where T : class, IConfigured, new();
    }

    public class ConfigurationResultStore : IConfigurationResultStore, IDisposable
    {
        public static object StoreLocker = new object();
        public static ReaderWriterLockSlim UpdateReadeWriterLock = new ReaderWriterLockSlim();

        private readonly IDictionary<Type, object> _results = new Dictionary<Type, object>();
        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        public void Store<T>(ConfigurationResult<T> configurationResult) where T : class, IConfigured, new()
        {
            UpdateResult(configurationResult.Result);
            lock (StoreLocker)
            {
                var sub = configurationResult.Observable.Subscribe(UpdateResult);
                _subscriptions.Add(sub);
            }
        }

        public void UpdateResult<T>(T newResult) where T : class, IConfigured, new()
        {
            try
            {
                UpdateReadeWriterLock.EnterWriteLock();
                _results[typeof(T)] = newResult;
            }
            finally
            {
                UpdateReadeWriterLock.ExitWriteLock();
            }
        }

        public T Get<T>() where T : class, IConfigured, new()
        {
            var key = typeof(T);

            try
            {
                UpdateReadeWriterLock.EnterReadLock();
                if (_results.ContainsKey(key))
                {
                    return (T)_results[key];
                }
            }
            finally
            {
                UpdateReadeWriterLock.ExitReadLock();
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
        }
    }
}
