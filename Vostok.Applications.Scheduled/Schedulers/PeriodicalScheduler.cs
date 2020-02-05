using System;
using JetBrains.Annotations;
using Vostok.Commons.Threading;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class PeriodicalScheduler : IScheduler
    {
        private readonly Func<TimeSpan> periodProvider;
        private readonly bool delayFirstIteration;
        private readonly double jitter;
        private readonly AtomicBoolean scheduledFirst;

        public PeriodicalScheduler([NotNull] Func<TimeSpan> periodProvider, bool delayFirstIteration, double jitter)
        {
            this.periodProvider = periodProvider ?? throw new ArgumentNullException(nameof(periodProvider));
            this.delayFirstIteration = delayFirstIteration;
            this.jitter = jitter;

            scheduledFirst = new AtomicBoolean(false);
        }

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            if (scheduledFirst.TrySetTrue() && !delayFirstIteration)
                return from;

            var period = periodProvider();

            if (jitter > 0d)
                period += period.Multiply(ThreadSafeRandom.NextDouble() * jitter * (ThreadSafeRandom.FlipCoin() ? 1d : -1d));

            return from + TimeSpanArithmetics.Max(TimeSpan.Zero, period);
        }
    }
}
