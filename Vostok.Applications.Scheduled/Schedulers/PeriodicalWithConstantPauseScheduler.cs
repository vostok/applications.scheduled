using System;
using JetBrains.Annotations;
using Vostok.Commons.Threading;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class PeriodicalWithConstantPauseScheduler : IScheduler, IScheduledActionEventListener
    {
        private readonly Func<TimeSpan> periodProvider;
        private readonly bool delayFirstIteration;
        private readonly AtomicBoolean scheduledFirst;

        private DateTimeOffset lastIterationEnd = PreciseDateTime.Now;

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

            return lastIterationEnd + TimeSpanArithmetics.Max(TimeSpan.Zero, periodProvider());
        }

        public void OnSuccessfulIteration(IScheduler source)
            => lastIterationEnd = PreciseDateTime.Now;

        public void OnFailedIteration(IScheduler source, Exception error)
            => lastIterationEnd = PreciseDateTime.Now;

        public override string ToString() => $"PeriodicalWithConstantPause({periodProvider()})";
    }
}
