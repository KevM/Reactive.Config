using System;
using System.Collections.Generic;
using System.Threading;

namespace Reactive.Config.Tests
{
    public static class ObservableTestExtensions
    {
        public static T[] Capture<T>(this IObservable<T> observable, int captureCount, int waitInSeconds = 1)
        {
            var result = new List<T>();
            var reset = new ManualResetEventSlim();

            var sub = observable.Subscribe(t =>
            {
                result.Add(t);
                if (result.Count < captureCount)
                {
                    reset.Set();
                }
            });

            reset.Wait(TimeSpan.FromSeconds(waitInSeconds));
            sub.Dispose();

            return result.ToArray();
        }

    }
}