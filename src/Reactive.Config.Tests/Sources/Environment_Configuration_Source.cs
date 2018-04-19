using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Reactive.Config.Sources;

namespace Reactive.Config.Tests.Sources
{
    public class Environment_Configuration_Source
    {
        [TestFixture]
        public class Get
        {
            private EnvironmentConfigurationSource _cut;
            private IKeyPathProvider _keyPathProvider;

            [SetUp]
            public void beforeAll()
            {
                _keyPathProvider = Substitute.For<IKeyPathProvider>();
                _keyPathProvider.GetKeyPath<TestConfigured>().Returns("TestConfigured");

                _cut = new EnvironmentConfigurationSource(_keyPathProvider, new PropertyValueBinder());
            }

            [Test]
            public void should_get_default_result_when_matching_environment_variable_is_missing()
            {
                var result = _cut.Get(ConfigurationResult<TestConfigured>.Create());

                AssertDefaultValue(result);
            }

            private static void AssertDefaultValue(ConfigurationResult<TestConfigured> result)
            {
                var defaultTestConfigured = new TestConfigured();

                result.Result.IsEnabled.Should().Be(defaultTestConfigured.IsEnabled);
                result.Result.AString.Should().Be(defaultTestConfigured.AString);
                result.Result.AnInt32.Should().Be(defaultTestConfigured.AnInt32);
            }

            [Test]
            public void should_get_value_from_environment_variable()
            {
                const string expectedString = "test string";
                const int expectedInt = -1234;
                Environment.SetEnvironmentVariable("TestConfigured.IsEnabled", "false");
                Environment.SetEnvironmentVariable("TestConfigured.AString", expectedString);
                Environment.SetEnvironmentVariable("TestConfigured.AnInt32", expectedInt.ToString());

                var result = _cut.Get(ConfigurationResult<TestConfigured>.Create());

                result.Result.IsEnabled.Should().BeFalse();
                result.Result.AString.Should().Be(expectedString);
                result.Result.AnInt32.Should().Be(expectedInt);
            }

            [Test]
            public void should_return_type_defaults_when_variable_cannot_be_bound_to_property()
            {
                Environment.SetEnvironmentVariable("TestConfigured.IsEnabled", "weeeeeeeeee");
                Environment.SetEnvironmentVariable("TestConfigured.AnInt32", "12345678901234556789012345512312313");

                var result = _cut.Get(ConfigurationResult<TestConfigured>.Create());

                result.Result.IsEnabled.Should().Be(default(bool));
                result.Result.AnInt32.Should().Be(default(int));
            }
        }
    }
}
