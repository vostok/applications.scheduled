using System;
using System.Collections.Generic;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class FixedScheduler : IScheduler
    {
        private readonly Func<IReadOnlyList<DateTimeOffset>> datesProvider;

        public FixedScheduler(Func<IReadOnlyList<DateTimeOffset>> datesProvider)
            => this.datesProvider = datesProvider ?? throw new ArgumentNullException(nameof(datesProvider));

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            var nearest = null as DateTimeOffset?;

            foreach (var date in datesProvider())
            {
                if (date <= from)
                    continue;

                if (nearest == null || date < nearest)
                    nearest = date;
            }

            return nearest;
        }
    }
}
