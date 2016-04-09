using System;
using Kralizek.ThrottledSection;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Tests
{
    public class TestClock : IClock
    {
        private DateTimeOffset _current;

        public TestClock(DateTimeOffset start)
        {
            _current = start;
        }

        public DateTimeOffset Now => _current;

        public void AdvanceBy(TimeSpan interval)
        {
            _current = _current + interval;
        }

        public void SetTo(DateTimeOffset time)
        {
            _current = time;
        }
    }

    [TestFixture]
    public class TestClockTests : TestBase<TestClock>
    {
        protected override TestClock CreateSystemUnderTest()
        {
            return new TestClock(Now);
        }

        [Test]
        public void Now_is_same_as_initial_value()
        {
            var sut = CreateSystemUnderTest();

            Assert.That(sut.Now, Is.EqualTo(Now));
        }

        [Test]
        public void Now_is_advanced_by_interval()
        {
            var sut = CreateSystemUnderTest();
            var interval = Fixture.Create<TimeSpan>();

            sut.AdvanceBy(interval);

            Assert.That(sut.Now, Is.EqualTo(Now + interval));
        }

        [Test]
        public void Now_is_set_to_value()
        {
            var sut = CreateSystemUnderTest();
            var newValue = Fixture.Create<DateTimeOffset>();

            sut.SetTo(newValue);

            Assert.That(sut.Now, Is.EqualTo(newValue));
        }
    }
}
