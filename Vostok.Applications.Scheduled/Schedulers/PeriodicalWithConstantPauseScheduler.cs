using System;
using JetBrains.Annotations;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class PeriodicalWithConstantPauseScheduler : IScheduler
    {
        private readonly Func<TimeSpan> periodProvider;

        public PeriodicalWithConstantPauseScheduler([NotNull] Func<TimeSpan> periodProvider)
            => this.periodProvider = periodProvider ?? throw new ArgumentNullException(nameof(periodProvider));

        public DateTimeOffset? ScheduleNext(DateTimeOffset from) => PreciseDateTime.Now + TimeSpanArithmetics.Max(TimeSpan.Zero, periodProvider());
    }
}
