using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kralizek.ThrottledSection;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Tests
{
    [TestFixture]
    public class ThrottledSectionTests : TestBase
    {
        private TestClock _testClock;

        [SetUp]
        public void Initialize()
        {
            _testClock = new TestClock(Now);
        }


        protected ThrottledSection CreateSystemUnderTest(int spots, TimeSpan interval)
        {
            return new ThrottledSection(spots, interval) {Clock = _testClock};
        }

        private ThrottledSection Fill(ThrottledSection section)
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
            var interval = Fixture.Create<TimeSpan>();

            var sut = CreateSystemUnderTest(1, interval);

            sut.TryEnter();

            Assert.That(sut.TryEnter(), Is.False);
        }

        [Test]
        public void TryEnter_returns_false_if_section_is_full_after_N_inserts()
        {
            var interval = Fixture.Create<TimeSpan>();
            var spots = Fixture.Create<int>();
            var sut = Fill(CreateSystemUnderTest(spots, interval));

            Assert.That(sut.TryEnter(), Is.False);
        }

        [Test]
        public void TryEnter_returns_true_if_interval_is_awaited_after_last_insert()
        {
            var interval = Fixture.Create<TimeSpan>();
            var spots = Fixture.Create<int>();
            var sut = Fill(CreateSystemUnderTest(spots, interval));

            // Wait longer than interval
            _testClock.AdvanceBy(interval + interval);

            Assert.That(sut.TryEnter(), Is.True);
        }

        [Test]
        public void HasSpots_returns_true_if_interval_is_awaited_after_last_insert()
        {
            var interval = Fixture.Create<TimeSpan>();
            var sut = Fill(CreateSystemUnderTest(1, interval));

            // Wait longer than interval
            _testClock.AdvanceBy(interval + interval);

            Assert.That(sut.CanEnter());
        }
    }
}
