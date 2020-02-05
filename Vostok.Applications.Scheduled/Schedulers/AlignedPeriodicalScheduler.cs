using System;
using JetBrains.Annotations;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class AlignedPeriodicalScheduler : IScheduler
    {
        private readonly Func<TimeSpan> periodProvider;

        public AlignedPeriodicalScheduler([NotNull] Func<TimeSpan> periodProvider)
            => this.periodProvider = periodProvider ?? throw new ArgumentNullException(nameof(periodProvider));

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            var period = TimeSpanArithmetics.Max(TimeSpan.Zero, periodProvider());
            if (period == TimeSpan.Zero)
                return from;

            var delayToNext = period - (from.Ticks % period.Ticks).Ticks();

            return from + delayToNext;
        }
    }
}
