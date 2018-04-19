using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Reactive.Config.Tests
{
    public class TestConfigured : IConfigured
    {
        public bool IsEnabled { get; set; } = true;
        public string AString { get; set; } = "this is a string";
        public int AnInt32 { get; set; } = 123456789;
    }

    public class Configuration_Provider
    {
        [TestFixture]
        public class Get
        {
            private ConfigurationProvider _cut;
            private IConfigurationResultStore _resultStore;
            private IConfigurationSourceResolver _sourceResolver;

            [SetUp]
            public void beforeAll()
            {
                _resultStore = Substitute.For<IConfigurationResultStore>();
                _sourceResolver = Substitute.For<IConfigurationSourceResolver>();

                _cut = new ConfigurationProvider(_resultStore, _sourceResolver);
            }

            [Test]
            public void should_check_result_store_for_existing_result()
            {
                var expectedResult = new TestConfigured();
                _resultStore.Get<TestConfigured>().Returns(expectedResult);

                var result = _cut.Get<TestConfigured>();

                result.Should().BeSameAs(expectedResult);
                _sourceResolver.DidNotReceive().Resolve<TestConfigured>();
            }
            
            [Test]
            public void should_store_aggregated_result_for_T()
            {
                var resolved = ConfigurationResult<TestConfigured>.Create();
                _sourceResolver.Resolve<TestConfigured>().Returns(resolved);

                _cut.Get<TestConfigured>();

                _resultStore.Received().Store(resolved);
            }
        }
    }
}
