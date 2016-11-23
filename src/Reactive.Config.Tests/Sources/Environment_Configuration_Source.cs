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

                //TODO setup or fake environment

                _cut = new EnvironmentConfigurationSource(_keyPathProvider);
            }

            [Test]
            public void should_check_result_store_for_existing_result()
            {
                var result = _cut.Get(ConfigurationResult<TestConfigured>.Create());
            }
        }
    }
}