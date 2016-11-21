using System;

namespace Reactive.Config
{
    public interface IConfigured {}

    public class MyExampleConfiguration : IConfigured {
        public bool IsEnabled { get;set;} = false;
        public DateTime EnabledOn { get;set;} = new DateTime(2017, 1, 1, 10, 30, 0);
        public string ShutoffMessage { get;set;} = "Sorry this feature has been disabled";
    }
}
