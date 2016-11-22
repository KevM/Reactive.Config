using System;

namespace Reactive.Config.Files.Sources
{
    public class JsonConfigurationSourceSettings
    {
        public string ConfigurationFilePath { get; set; } = Environment.CurrentDirectory;
        public double PollingIntervalInSeconds { get; set; } = 5.0;
    }
}