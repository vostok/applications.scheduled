using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Tests
{
    [TestFixture]
    internal class FixedScheduler_Tests
    {
        [Test]
        public void Should_return_null_when_there_are_no_dates()
            => ScheduleFromNow().Should().BeNull();

        [Test]
        public void Should_return_null_when_no_dates_are_in_future()
            => ScheduleFromNow(-1.Seconds(), -5.Days()).Should().BeNull();

        [Test]
        public void Should_return_nearest_of_the_future_dates()
            => ScheduleFromNow(3.Days(), 2.Days(), 1.Days()).Should().Be(1.Days());

        private TimeSpan? ScheduleFromNow(params TimeSpan[] offsetsFromNow)
        {
            var now = PreciseDateTime.Now;
            var dates = offsetsFromNow.Select(offset => now + offset).ToArray();

            return Scheduler.Fixed(dates).ScheduleNext(DateTimeOffset.Now) - now;
        }
    }
}
