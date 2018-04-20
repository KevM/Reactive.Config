using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;

namespace Reactive.Config
{
    /// <summary>
    /// Resolved configured types are held in memory here to avoid redundant re-population. 
    /// Each result contians an <see cref="Observable"/> to which this store subscribes 
    /// to remain up-to-date with the current state of the configured type.
    /// </summary>
    public interface IConfigurationResultStore
    {
        /// <summary>
        /// Store the configured result and track its changes.
        /// </summary>
        void Store<T>(ConfigurationResult<T> configurationResult) where T : class, IConfigured, new();

        /// <summary>
        /// Retrieve an up-to-date version of the configured result.
        /// </summary>
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

        /// <summary>
        /// This method is invoked by observations of changes to the configuration result. 
        /// </summary>
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
