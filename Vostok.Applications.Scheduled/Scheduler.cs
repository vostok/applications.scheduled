using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Schedulers;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public static class Scheduler
    {
        public static IScheduler Periodical([NotNull] Func<TimeSpan> periodProvider, double jitter, bool delayFirstIteration = true)
            => new PeriodicalScheduler(periodProvider, delayFirstIteration, jitter);

        public static IScheduler Periodical([NotNull] Func<TimeSpan> periodProvider, bool delayFirstIteration = true)
            => new PeriodicalScheduler(periodProvider, delayFirstIteration, 0d);

        public static IScheduler Periodical(TimeSpan period, double jitter, bool delayFirstIteration = true)
            => new PeriodicalScheduler(() => period, delayFirstIteration, jitter);

        public static IScheduler Periodical(TimeSpan period, bool delayFirstIteration = true)
            => new PeriodicalScheduler(() => period, delayFirstIteration, 0d);

        public static IScheduler AlignedPeriodical([NotNull] Func<TimeSpan> periodProvider)
            => new AlignedPeriodicalScheduler(periodProvider);

        public static IScheduler AlignedPeriodical(TimeSpan period)
            => new AlignedPeriodicalScheduler(() => period);

        public static IScheduler Crontab([NotNull] Func<string> scheduleProvider)
            => new CrontabScheduler(scheduleProvider);

        public static IScheduler Crontab([NotNull] string schedule)
            => new CrontabScheduler(() => schedule);

        public static IScheduler Fixed([NotNull] Func<IReadOnlyList<DateTimeOffset>> datesProvider)
            => new FixedScheduler(datesProvider);

        public static IScheduler Fixed([NotNull] IReadOnlyList<DateTimeOffset> dates)
            => new FixedScheduler(() => dates);

        public static IScheduler Fixed([NotNull] Func<DateTimeOffset> dateProvider)
            => new FixedScheduler(() => new [] {dateProvider()});

        public static IScheduler Fixed(DateTimeOffset date)
            => new FixedScheduler(() => new [] {date});

        public static IScheduler OnDemand(out Action demand)
        {
            var scheduler = new OnDemandScheduler();
            demand = scheduler.Demand;
            return scheduler;
        }
    }
}
