
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Reactive.Config.Tests
{
    public class TestConfigured : IConfigured
    {
        public bool IsEnabled { get; set; } = true;
    }

    public class Configuration_Provider
    {
        [TestFixture]
        public class Get
        {
            private ConfigurationProvider _cut;
            private IConfigurationSource[] _sources;
            private IConfigurationResultStore _resultStore;

            [SetUp]
            public void beforeAll()
            {
                _resultStore = Substitute.For<IConfigurationResultStore>();

                var source1 = Substitute.For<IConfigurationSource>();
                var source2 = Substitute.For<IConfigurationSource>();
                _sources = new[] {source1, source2};
                
                _cut = new ConfigurationProvider(_sources, _resultStore);
            }

            [Test]
            public void should_check_result_store_for_existing_result()
            {
                var expectedResult = new TestConfigured();
                _resultStore.Get<TestConfigured>().Returns(expectedResult);

                var result = _cut.Get<TestConfigured>();

                result.Should().BeSameAs(expectedResult);
            }
            
            [Test]
            public void should_ask_each_source_if_it_handles_T()
            {
                _cut.Get<TestConfigured>();

                _sources[0].Received().Handles<TestConfigured>();
                _sources[1].Received().Handles<TestConfigured>();
            }

            [Test]
            public void should_get_results_from_sources_handling_T()
            {
                var expectedResult = new TestConfigured();
                var configurationResult = new ConfigurationResult<TestConfigured>(expectedResult);
                _sources[1].Handles<TestConfigured>().Returns(true);
                _sources[1]
                    .Get(Arg.Any<ConfigurationResult<TestConfigured>>())
                    .Returns(configurationResult);

                _cut.Get<TestConfigured>();

                _sources[0].DidNotReceive()
                    .Get(Arg.Any<ConfigurationResult<TestConfigured>>());
                _sources[1].Received()
                    .Get(Arg.Any<ConfigurationResult<TestConfigured>>());
            }

            [Test]
            public void should_return_aggregated_result_from_sources_handling_T()
            {
                var configurationResult1 = new ConfigurationResult<TestConfigured>(new TestConfigured());
                _sources[0].Handles<TestConfigured>().Returns(true);
                _sources[0].Get(Arg.Any<ConfigurationResult<TestConfigured>>())
                    .Returns(configurationResult1);

                var expectedResult = new TestConfigured();
                var configurationResult2 = new ConfigurationResult<TestConfigured>(expectedResult);
                _sources[1].Handles<TestConfigured>().Returns(true);
                _sources[1].Get(configurationResult1)
                    .Returns(configurationResult2);

                var result = _cut.Get<TestConfigured>();

                _sources[0].Received().Get(Arg.Any<ConfigurationResult<TestConfigured>>());
                _sources[1].Received().Get(configurationResult1);

                result.Should().BeSameAs(expectedResult);
            }

            [Test]
            public void should_store_aggregated_result_for_T()
            {
                var aggregatedResult = new ConfigurationResult<TestConfigured>(new TestConfigured());
                _sources[0].Handles<TestConfigured>().Returns(true);
                _sources[0].Get(Arg.Any<ConfigurationResult<TestConfigured>>())
                    .Returns(aggregatedResult);

                _cut.Get<TestConfigured>();

                _resultStore.Received().Store(aggregatedResult);
            }
        }
    }
}