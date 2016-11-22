using System;
using System.IO;

namespace Reactive.Config.Files.Watcher
{
    public class Notification
    {
        public enum Type
        {
            None,
            Created,
            Changed,
            Missing,
            Error
        }

        public FileInfo Info { get; }
        public bool Exists { get; }
        public long Size { get; }
        public Type NotificationType { get; }
        public Exception Error { get; }

        public Notification(FileInfo fileInfo)
        {
            fileInfo.Refresh();
            Info = fileInfo;
            Exists = fileInfo.Exists;

            if (Exists)
            {
                NotificationType = Type.None;
                Size = Info.Length;
            }
            else
            {
                NotificationType = Type.Missing;
            }
        }

        public Notification(FileInfo fileInfo, Exception error)
        {
            Info = fileInfo;
            Error = error;
            Exists = false;
            NotificationType = Type.Error;
        }

        public Notification(Notification previous)
        {
            previous.Info.Refresh();

            Info = previous.Info;
            Exists = Info.Exists;

            if (!Exists)
            {
                NotificationType = Type.Missing;
                return;
            }

            Size = Info.Length;

            if (!previous.Exists)
            {
                NotificationType = Type.Created;
            }
            else if (Size != previous.Size)
            {
                NotificationType = Type.Changed;
            }
            else
            {
                NotificationType = Type.None;
            }
        }

        public bool Equals(Notification other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Info.FullName, other.Info.FullName)
                   && Exists == other.Exists
                   && Size == other.Size
                   && NotificationType == other.NotificationType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Notification) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Info.FullName.GetHashCode();
                hashCode = (hashCode*397) ^ Exists.GetHashCode();
                hashCode = (hashCode*397) ^ Size.GetHashCode();
                hashCode = (hashCode*397) ^ (int) NotificationType;
                return hashCode;
            }
        }
    }
}
