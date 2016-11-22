using System;
using Reactive.Config;

namespace HelloReactiveConfig
{
    public class MyConfigured : IConfigured, IEquatable<MyConfigured>
    {
        public bool IsEnabled { get; set; } = true;
        public DateTime EnabledOn { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(-7);
        public string MyAppKey { get; set; } = "sooooouuuuuper seeeecrette app key";

        public bool Equals(MyConfigured other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEnabled == other.IsEnabled && EnabledOn.Equals(other.EnabledOn) && string.Equals(MyAppKey, other.MyAppKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyConfigured)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsEnabled.GetHashCode();
                hashCode = (hashCode * 397) ^ EnabledOn.GetHashCode();
                hashCode = (hashCode * 397) ^ MyAppKey.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"My configured: enabled: {IsEnabled}, enabled on: {EnabledOn}, app key: {MyAppKey}";
        }
    }
}
