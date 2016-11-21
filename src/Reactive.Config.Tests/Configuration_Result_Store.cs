
using System.Reactive.Subjects;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NUnit.Framework;

namespace Reactive.Config.Tests
{
    public class Configuration_Result_Store
    {
        [TestFixture]
        public class Happy_Path
        {
            private ConfigurationResultStore _cut;
            private Subject<TestConfigured> _observable;

            [SetUp]
            public void beforeAll()
            {
                _cut = new ConfigurationResultStore();
                _observable = new Subject<TestConfigured>();
            }

            [Test]
            public void initial_get_before_store_should_return_null()
            {
                _cut.Get<TestConfigured>().Should().BeNull();
            }

            [Test]
            public void get_after_store_should_match_stored_result()
            {
                var original = new TestConfigured();
                var configResult = new ConfigurationResult<TestConfigured>(original)
                {
                    Observable = _observable
                };
                _cut.Store(configResult);

                _cut.Get<TestConfigured>().Should().BeSameAs(original);
            }

            [Test]
            public void get_after_observable_fires_should_return_observed()
            {
                var original = new TestConfigured();
                var replacement = new TestConfigured();
                var configResult = new ConfigurationResult<TestConfigured>(original)
                {
                    Observable = _observable
                };
                _cut.Store(configResult);
                _observable.OnNext(replacement);

                _cut.Get<TestConfigured>().Should().BeSameAs(replacement);
            }

            [Test, Explicit]
            public void disposing_the_store_should_unsubscribe_all()
            {
                //TODO look around for my old RX IObservable mocking helper to see if I can better test this.
            }


            [Test, Explicit]
            public void should_we_bother_testing_locking()
            {
                //TODO look around for my old RX IObservable mocking helper to see if I can better test this.
            }
        }
    }
}