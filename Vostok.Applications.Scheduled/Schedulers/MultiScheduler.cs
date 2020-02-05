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
        {
            var nearest = null as DateTimeOffset?;

            foreach (var scheduler in schedulers)
            {
                var next = scheduler.ScheduleNext(from);

                if (nearest == null || next < nearest)
                    nearest = next;
            }

            return nearest;
        }
    }
}
