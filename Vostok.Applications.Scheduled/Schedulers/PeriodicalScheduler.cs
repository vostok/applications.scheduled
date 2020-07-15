using System;
using JetBrains.Annotations;
using Vostok.Commons.Threading;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class PeriodicalScheduler : IScheduler
    {
        private readonly Func<TimeSpan> periodProvider;
        private readonly Func<double> jitterProvider;
        private readonly bool delayFirstIteration;
        private readonly AtomicBoolean scheduledFirst;

        public PeriodicalScheduler(
            [NotNull] Func<TimeSpan> periodProvider, 
            [NotNull] Func<double> jitterProvider, 
            bool delayFirstIteration)
        {
            this.periodProvider = periodProvider ?? throw new ArgumentNullException(nameof(periodProvider));
            this.jitterProvider = jitterProvider ?? throw new ArgumentNullException(nameof(jitterProvider));
            this.delayFirstIteration = delayFirstIteration;

            scheduledFirst = new AtomicBoolean(false);
        }

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            if (!delayFirstIteration && scheduledFirst.TrySetTrue())
                return from;

            var period = periodProvider();
            var jitter = jitterProvider();

            period = TimeSpanArithmetics.Max(TimeSpan.Zero, period);
            jitter = Math.Max(0d, jitter);
            jitter = Math.Min(1d, jitter);

            var next = from + period;

            if (jitter > 0d)
                next += period.Multiply(ThreadSafeRandom.NextDouble() * jitter * (ThreadSafeRandom.FlipCoin() ? 1d : -1d));

            return next;
        }

        public override string ToString() => $"Periodical({periodProvider()})";
    }
}
