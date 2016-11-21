using System;
using Reactive.Config;

namespace HelloReactiveConfig
{
    public class MyConfigured : IConfigured
    {
        public bool IsEnabled { get; set; }
        public DateTime EnabledOn { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(7);

        public string MyAppKey { get; set; } = "sooooouuuuuper seeeecrette app key";
    }
}
