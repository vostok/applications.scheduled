using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Tests
{
    [TestFixture]
    internal class AlignedPeriodicalScheduler_Tests
    {
        private DateTimeOffset from;

        [SetUp]
        public void TestSetup()
            => from = new DateTimeOffset(1900, 5, 4, 15, 32, 21, 151, TimeSpan.Zero);

        [Test]
        public void Should_align_to_milliseconds()
        {
            Scheduler.AlignedPeriodical(1.Milliseconds()).ScheduleNext(from).Should().Be(from + 1.Milliseconds());
            Scheduler.AlignedPeriodical(5.Milliseconds()).ScheduleNext(from).Should().Be(from + 4.Milliseconds());
            Scheduler.AlignedPeriodical(10.Milliseconds()).ScheduleNext(from).Should().Be(from + 9.Milliseconds());
        }

        [Test]
        public void Should_align_to_seconds()
        {
            Scheduler.AlignedPeriodical(1.Seconds()).ScheduleNext(from).Should().Be(from + 849.Milliseconds());
            Scheduler.AlignedPeriodical(5.Seconds()).ScheduleNext(from).Should().Be(from + 849.Milliseconds() + 3.Seconds());
            Scheduler.AlignedPeriodical(15.Seconds()).ScheduleNext(from).Should().Be(from + 849.Milliseconds() + 8.Seconds());
        }

        [Test]
        public void Should_align_to_minutes()
            => Scheduler.AlignedPeriodical(2.Minutes()).ScheduleNext(from).Should().Be(from + 849.Milliseconds() + 38.Seconds() + 1.Minutes());

        [Test]
        public void Should_align_to_hours()
            => Scheduler.AlignedPeriodical(3.Hours()).ScheduleNext(from).Should().Be(from + 849.Milliseconds() + 38.Seconds() + 27.Minutes() + 2.Hours());

        [Test]
        public void Should_align_to_days()
            => Scheduler.AlignedPeriodical(1.Days()).ScheduleNext(from).Should().Be(from + 849.Milliseconds() + 38.Seconds() + 27.Minutes() + 8.Hours());

        [Test]
        public void Should_schedule_immediately_for_zero_period()
            => Scheduler.AlignedPeriodical(TimeSpan.Zero).ScheduleNext(from).Should().Be(from);

        [Test]
        public void Should_schedule_immediately_for_negative_period()
            => Scheduler.AlignedPeriodical(-1.Seconds()).ScheduleNext(from).Should().Be(from);
    }
}
