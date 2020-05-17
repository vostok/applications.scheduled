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

        [Test]
        public void Should_unwrap_element_schedulers()
        {
            var now = PreciseDateTime.Now;

            var scheduler1 = new FixedScheduler(() => new []{ now + 2.Minutes() });
            var scheduler2 = new FixedScheduler(() => new []{ now + 4.Minutes() });
            var scheduler3 = new FixedScheduler(() => new []{ now + 1.Minutes() });
            var scheduler4 = new FixedScheduler(() => new []{ now + 3.Minutes() });

            var multi1 = Scheduler.Multi(scheduler1, scheduler2);
            var multi2 = Scheduler.Multi(scheduler3, scheduler4);

            var (next, source) = Scheduler.Multi(multi1, multi2).ScheduleNextWithSource(now);

            next.Should().Be(now + 1.Minutes());
            source.Should().BeSameAs(scheduler3);
        }

        private static TimeSpan? ScheduleFromNow(params TimeSpan?[] offsetsFromNow)
        {
            var now = PreciseDateTime.Now;
            var dates = offsetsFromNow.Select(offset => now + offset).ToArray();
            var schedulers = dates.Select(
                date => date == null
                    ? new FixedScheduler(Array.Empty<DateTimeOffset>)
                    : new FixedScheduler(() => new[] {date.Value}))
                .Cast<IScheduler>()
                .ToArray();

            return Scheduler.Multi(schedulers).ScheduleNext(DateTimeOffset.Now) - now;
        }
    }
}
