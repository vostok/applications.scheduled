using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Applications.Scheduled.Schedulers;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Tests
{
    [TestFixture]
    internal class MultiScheduler_Tests
    {
        [Test]
        public void Should_return_null_when_there_are_no_schedulers()
            => ScheduleFromNow().Should().BeNull();

        [Test]
        public void Should_return_null_when_all_schedulers_return_null()
            => ScheduleFromNow(null, null, null).Should().BeNull();

        [Test]
        public void Should_return_nearest_of_the_future_dates()
            => ScheduleFromNow(3.Days(), null, 1.Days(), null, 2.Days()).Should().Be(1.Days());

        private TimeSpan? ScheduleFromNow(params TimeSpan?[] offsetsFromNow)
        {
            var now = PreciseDateTime.Now;
            var dates = offsetsFromNow.Select(offset => now + offset).ToArray();
            var schedulers = dates.Select(
                date => date == null
                    ? new FixedScheduler(() => Array.Empty<DateTimeOffset>())
                    : new FixedScheduler(() => new[] {date.Value}))
                .Cast<IScheduler>()
                .ToArray();

            return Scheduler.Multi(schedulers).ScheduleNext(DateTimeOffset.Now) - now;
        }
    }
}
