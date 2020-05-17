using System;
using System.Collections.Generic;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class MultiScheduler : IScheduler
    {
        private readonly IReadOnlyList<IScheduler> schedulers;

        public MultiScheduler(IReadOnlyList<IScheduler> schedulers)
            => this.schedulers = schedulers ?? throw new ArgumentNullException(nameof(schedulers));

        public DateTimeOffset? ScheduleNext(DateTimeOffset from) 
            => ScheduleNextWithSource(from).timestamp;

        public (DateTimeOffset? timestamp, IScheduler source) ScheduleNextWithSource(DateTimeOffset from)
        {
            var nearest = null as DateTimeOffset?;
            var nearestScheduler = null as IScheduler;

            foreach (var scheduler in schedulers)
            {
                var (next, nextScheduler) = scheduler.ScheduleNextWithSource(from);

                if (nearest == null || next < nearest)
                {
                    nearest = next;
                    nearestScheduler = nextScheduler;
                }
            }

            return (nearest, nearestScheduler);
        }
    }
}
