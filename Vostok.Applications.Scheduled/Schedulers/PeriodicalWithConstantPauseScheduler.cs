using System;
using JetBrains.Annotations;
using Vostok.Commons.Threading;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class PeriodicalWithConstantPauseScheduler : IScheduler
    {
        private readonly Func<TimeSpan> periodProvider;
        private readonly bool delayFirstIteration;
        private readonly AtomicBoolean scheduledFirst;

        public PeriodicalWithConstantPauseScheduler([NotNull] Func<TimeSpan> periodProvider, bool delayFirstIteration)
        {
            this.periodProvider = periodProvider ?? throw new ArgumentNullException(nameof(periodProvider));
            this.delayFirstIteration = delayFirstIteration;

            scheduledFirst = new AtomicBoolean(false);
        }

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            if (!delayFirstIteration && scheduledFirst.TrySetTrue())
                return from;

            return PreciseDateTime.Now + TimeSpanArithmetics.Max(TimeSpan.Zero, periodProvider());
        }

        public override string ToString() => $"PeriodicalWithConstantPause({periodProvider()})";
    }
}
