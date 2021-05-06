using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledActionsDynamicOptions
    {
        public ScheduledActionsDynamicOptions(Func<IScheduledActionsBuilder, CancellationToken, Task> setup)
            => Setup = setup;

        public Func<IScheduledActionsBuilder, CancellationToken, Task> Setup { get; }

        public TimeSpan ActualizationPeriod { get; set; } = 10.Seconds();
    }
}