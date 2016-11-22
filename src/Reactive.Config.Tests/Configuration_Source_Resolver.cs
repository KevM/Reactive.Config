using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Reactive.Config.Tests
{
    public class Configuration_Source_Resolver
    {
        [TestFixture]
        public class Resolve
        {
            private ConfigurationSourceResolver _cut;
            private IConfigurationSource[] _sources;

            [SetUp]
            public void beforeAll()
            {
                var source1 = Substitute.For<IConfigurationSource>();
                var source2 = Substitute.For<IConfigurationSource>();
                _sources = new[] { source1, source2 };

                _cut = new ConfigurationSourceResolver(_sources);
            }

            [Test]
            public void should_ask_each_source_if_it_handles_T()
            {
                _cut.Resolve<TestConfigured>();

                _sources[0].Received().Handles<TestConfigured>();
                _sources[1].Received().Handles<TestConfigured>();
            }

            [Test]
            public void should_get_results_from_sources_handling_T()
            {
                var expectedResult = new TestConfigured();
                var configurationResult = ConfigurationResult<TestConfigured>.Create(expectedResult);
                _sources[1].Handles<TestConfigured>().Returns(true);
                _sources[1]
                    .Get(Arg.Any<ConfigurationResult<TestConfigured>>())
                    .Returns(configurationResult);

                _cut.Resolve<TestConfigured>();

                _sources[0].DidNotReceive()
                    .Get(Arg.Any<ConfigurationResult<TestConfigured>>());
                _sources[1].Received()
                    .Get(Arg.Any<ConfigurationResult<TestConfigured>>());
            }

            [Test]
            public void should_return_aggregated_result_from_sources_handling_T()
            {
                var result1 = ConfigurationResult<TestConfigured>.Create();
                _sources[0].Handles<TestConfigured>().Returns(true);
                _sources[0].Get(Arg.Any<ConfigurationResult<TestConfigured>>())
                    .Returns(result1);

                var expectedResult = new TestConfigured();
                var result2 = ConfigurationResult<TestConfigured>.Create(expectedResult);
                _sources[1].Handles<TestConfigured>().Returns(true);
                _sources[1].Get(result1)
                    .Returns(result2);

                var configurationResult = _cut.Resolve<TestConfigured>();

                _sources[0].Received().Get(Arg.Any<ConfigurationResult<TestConfigured>>());
                _sources[1].Received().Get(result1);

                configurationResult.Result.Should().BeSameAs(expectedResult);
            }

            [Test]
            public void should_return_merged_observable()
            {
                var observableResult1 = new TestConfigured();
                var observable1 = Observable.Return(observableResult1);
                var configurationResult1 = ConfigurationResult<TestConfigured>.Create(observable: observable1);
                _sources[0].Handles<TestConfigured>().Returns(true);
                _sources[0].Get(Arg.Any<ConfigurationResult<TestConfigured>>())
                    .Returns(configurationResult1);

                var observableResult2 = new TestConfigured();
                var observable2 = Observable.Return(observableResult2);
                var configurationResult2 = ConfigurationResult<TestConfigured>.Create(observable: observable2);
                _sources[1].Handles<TestConfigured>().Returns(true);
                _sources[1].Get(configurationResult1)
                    .Returns(configurationResult2);


                var configurationResult = _cut.Resolve<TestConfigured>();

                var captured = configurationResult.Observable.Capture(2);
                captured[0].Should().Be(observableResult1);
                captured[1].Should().Be(observableResult2);
            }
        }
    }
}