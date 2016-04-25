using System;
using Kralizek.ThrottledSection;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Tests
{
    [TestFixture]
    public class FixedResetThrottledSectionTests : TestBase
    {
        private TestClock _testClock;
        private DateTimeOffset _resetTime;

        [SetUp]
        public void Initialize()
        {
            _testClock = new TestClock(Now);

            _resetTime = Now.AddHours(-1);
        }

        private FixedResetThrottledSection CreateSystemUnderTest(int spots, TimeSpan interval)
        {
            return new FixedResetThrottledSection(spots, _resetTime, interval) { Clock = _testClock };
        }

        private IThrottledSection Fill(IThrottledSection section)
        {
            do
            {
                section.TryEnter();
            } while (section.CanEnter());

            return section;
        }

        [Test]
        public void TryEnter_returns_false_if_section_is_full_after_a_insert()
        {
            var minutes = Fixture.Create<int>();
            var interval = TimeSpan.FromMinutes(minutes);

            Assume.That(interval > TimeSpan.Zero);

            var sut = CreateSystemUnderTest(1, interval);

            sut.TryEnter();

            Assert.That(sut.TryEnter(), Is.False);
        }

        [Test]
        public void TryEnter_returns_false_if_section_is_full_after_N_inserts()
        {
            var minutes = Fixture.Create<int>();
            var interval = TimeSpan.FromMinutes(minutes);

            var spots = Fixture.Create<int>();
            var sut = Fill(CreateSystemUnderTest(spots, interval));

            Assume.That(sut.CanEnter(), Is.False);

            Assert.That(sut.TryEnter(), Is.False);
        }

        [Test]
        public void TryEnter_returns_true_if_interval_is_awaited_after_last_insert()
        {
            var minutes = Fixture.Create<int>();
            var interval = TimeSpan.FromMinutes(minutes);

            var spots = Fixture.Create<int>();

            var sut = CreateSystemUnderTest(spots, interval);

            Fill(sut);

            Assume.That(sut.CanEnter(), Is.False);

            // Wait longer than interval
            _testClock.AdvanceBy(interval + TimeSpan.FromTicks(1));

            Assert.That(sut.TryEnter(), Is.True);
        }

        [Test]
        public void CanEnter_returns_true_if_interval_is_awaited_after_last_insert()
        {
            var minutes = Fixture.Create<int>();
            var interval = TimeSpan.FromMinutes(minutes);

            var sut = Fill(CreateSystemUnderTest(1, interval));

            // Wait longer than interval
            _testClock.AdvanceBy(interval + interval + TimeSpan.FromTicks(1));

            Assert.That(sut.CanEnter());
        }

    }
}