using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledActionsDynamicOptions
    {
        public ScheduledActionsDynamicOptions(Func<IScheduledActionsBuilder, CancellationToken, Task> configuration, TimeSpan actualizationPeriod)
        {
            Configuration = configuration;
            ActualizationPeriod = actualizationPeriod;
        }

        public Func<IScheduledActionsBuilder, CancellationToken, Task> Configuration { get; }

        public TimeSpan ActualizationPeriod { get; }
    }
}