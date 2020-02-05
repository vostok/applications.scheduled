using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Tests
{
    [TestFixture]
    internal class PeriodicalScheduler_Tests
    {
        private DateTimeOffset from;

        [SetUp]
        public void TestSetup()
        {
            from = DateTimeOffset.Now;
        }

        [Test]
        public void Should_schedule_period_away_by_default()
        {
            var scheduler = Scheduler.Periodical(5.Seconds());

            scheduler.ScheduleNext(from).Should().Be(from + 5.Seconds());
            scheduler.ScheduleNext(from).Should().Be(from + 5.Seconds());
        }

        [Test]
        public void Should_not_delay_first_iteration_if_asked_to()
        {
            var scheduler = Scheduler.Periodical(5.Seconds(), false);

            scheduler.ScheduleNext(from).Should().Be(from);
            scheduler.ScheduleNext(from).Should().Be(from + 5.Seconds());
            scheduler.ScheduleNext(from).Should().Be(from + 5.Seconds());
        }

        [Test]
        public void Should_handle_zero_and_negative_period()
        {
            Scheduler.Periodical(TimeSpan.Zero).ScheduleNext(from).Should().Be(from);
            Scheduler.Periodical(-1.Seconds()).ScheduleNext(from).Should().Be(from);
        }

        [Test]
        public void Should_apply_jitter_if_asked_to()
        {
            var scheduler = Scheduler.Periodical(5.Seconds(), 0.1);
            var encounteredDifferent = false;

            for (var i = 0; i < 100; i++)
            {
                var expected = from + 5.Seconds();
                var next = scheduler.ScheduleNext(from);
                if (next != expected)
                    encounteredDifferent = true;

                next.Should().BeCloseTo(expected, 500.Milliseconds());
            }

            encounteredDifferent.Should().BeTrue();
        }
    }
}
