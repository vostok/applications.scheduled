using System;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Schedulers;

namespace Vostok.Applications.Scheduled
{
    internal static class ISchedulerExtensions
    {
        public static (DateTimeOffset? timestamp, IScheduler source) ScheduleNextWithSource(
            [NotNull] this IScheduler scheduler, DateTimeOffset from)
            => scheduler is MultiScheduler multiScheduler 
                ? multiScheduler.ScheduleNextWithSource(from) 
                : (scheduler.ScheduleNext(from), scheduler);
    }
}
