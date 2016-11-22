using System;
using System.IO;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Reactive.Config.Files.Sources;

namespace Reactive.Config.Files.Tests
{
    public class TestConfigured : IConfigured
    {
        public bool IsEnabled { get; set; } = true;
        public DateTime EnabledOn { get; set; } = DateTime.UtcNow;
        public string AppKey { get; set; } = "default app key";
    }

    public class Json_Configuration_Source
    {
        [TestFixture]
        public class Handles
        {
            private JsonConfigurationSourceSettings _settings;
            private JsonConfigurationSource _cut;
            private string _expectedNamespace;

            [SetUp]
            public void beforeEach()
            {
                _settings = CreateSettings();
                var keypath = Substitute.For<IKeyPathProvider>();
                _expectedNamespace = "namespace";
                keypath.GetKeyPath<TestConfigured>().Returns(_expectedNamespace);

                _cut = new JsonConfigurationSource(keypath, _settings);
            }

            [Test]
            public void should_handle_when_settings_path_for_T_exists()
            {
                var settingsPath = _cut.GetSettingsFileInfo<TestConfigured>();
                CreateConfigFile(settingsPath, "{}");

                var result = _cut.Handles<TestConfigured>();

                result.Should().BeTrue();
            }
            
            [Test]
            public void should_not_handle_when_settings_path_does_not_exist()
            {
                var result = _cut.Handles<TestConfigured>();

                result.Should().BeFalse();
            }

            [Test]
            public void should_not_handle_when_settings_file_does_not_exist()
            {
                Directory.CreateDirectory(_settings.SettingsFilePath);

                var result = _cut.Handles<TestConfigured>();

                result.Should().BeFalse();
            }
        }

        [TestFixture]
        public class Get
        {
            private JsonConfigurationSourceSettings _settings;
            private JsonConfigurationSource _cut;
            private string _expectedNamespace;

            [SetUp]
            public void beforeEach()
            {
                _settings = CreateSettings();
                var keypath = Substitute.For<IKeyPathProvider>();
                _expectedNamespace = "org.app.lib";
                keypath.GetKeyPath<TestConfigured>().Returns(_expectedNamespace);

                _cut = new JsonConfigurationSource(keypath, _settings);
            }

            [Test]
            public void should_return_deserialized_config_contents()
            {
                var model = new TestConfigured();
                _cut.CreateConfigFile(model);

                var configurationResult = _cut.Get(ConfigurationResult<TestConfigured>.Create());

                configurationResult.Result.AppKey.Should().Be(model.AppKey);
                configurationResult.Result.IsEnabled.Should().Be(model.IsEnabled);
                configurationResult.Result.EnabledOn.Should().Be(model.EnabledOn);
            }
        }

        public static JsonConfigurationSourceSettings CreateSettings()
        {
            var subdir = "f" + Guid.NewGuid().ToString().Replace("-", "");
            var settingsFilePath = Path.Combine(Path.GetTempPath(), subdir);
            return new JsonConfigurationSourceSettings
            {
                SettingsFilePath = settingsFilePath
            };
        }

        public static void CreateConfigFile(FileInfo file, string contents = "")
        {
            if (file.Directory == null) throw new ArgumentException("File has to be in a directory");
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            File.WriteAllText(file.FullName, contents);
        }
    }
}
