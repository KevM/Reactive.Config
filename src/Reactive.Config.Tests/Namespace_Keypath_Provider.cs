using FluentAssertions;
using NUnit.Framework;

namespace Reactive.Config.Tests
{
    public class Namespace_Keypath_Provider
    {
        [TestFixture]
        public class Get_Key_Path
        {
            private NamespaceKeyPathProvider _cut;

            [SetUp]
            public void beforeAll()
            {
                _cut = new NamespaceKeyPathProvider();
            }

            [Test]
            public void should_return_namespace_of_T()
            {
                var model = new TestConfigured();
                var type = model.GetType();

                _cut.GetKeyPath(model).Should().Be(type.Namespace);
            }
        }
    }
}