using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Reactive.Config.Files.Watcher
{
    public static class FileInfoExtensions
    {
        public static IObservable<Notification> PollFile(this FileInfo file, TimeSpan? refreshPeriod = null, IScheduler scheduler = null)
        {
            return Observable.Create<Notification>(observer =>
            {
                var localRefresh = refreshPeriod ?? TimeSpan.FromSeconds(5);
                var localSchedule = scheduler ?? Scheduler.Default;

                var notification = new Notification(file);
                return localSchedule.Schedule(localRefresh, scheduleNext =>
                {
                    try
                    {
                        notification = new Notification(notification);

                        if (notification.NotificationType == Notification.Type.None)
                        {
                            return;
                        }

                        observer.OnNext(notification);
                    }
                    catch (Exception ex)
                    {
                        notification = new Notification(file, ex);
                        observer.OnNext(notification);
                    }
                    finally
                    {
                        scheduleNext(localRefresh);
                    }
                });
            }).DistinctUntilChanged();
        }
    }
}