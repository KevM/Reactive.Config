using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;

namespace Reactive.Config.Files.Tests
{
    public static class ObservableTestExtensions
    {
        public static T CaptureFirst<T>(this IObservable<T> observable, Action action = null, double waitInSeconds = 1.0)
        {
            return Capture(observable, 1, action, waitInSeconds).First();
        }

        public static T[] Capture<T>(this IObservable<T> observable, int captureCount, Action action = null, double waitInSeconds = 1.0)
        {
            var result = new List<T>();
            var reset = new ManualResetEventSlim();

            var sub = observable.Subscribe(t =>
            {
                result.Add(t);
                if (result.Count >= captureCount)
                {
                    reset.Set();
                }
            });

            action?.Invoke();
            var success = reset.Wait(TimeSpan.FromSeconds(waitInSeconds));

            success
                .Should()
                .BeTrue($"Expected {captureCount} capture(s) but saw {result.Count} while waiting for {waitInSeconds} second(s).");

            sub.Dispose();

            return result.ToArray();
        }

        public static T Capture<T>(this IObservable<T> observable, double waitInSeconds = 1.0) where T : class
        {
            T result = null;
            var reset = new ManualResetEventSlim();

            var sub = observable.Subscribe(t =>
            {
                result = t;
                reset.Set();
            });

            reset.Wait(TimeSpan.FromSeconds(waitInSeconds));
            sub.Dispose();

            return result;
        }
    }
}