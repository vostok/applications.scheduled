using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledAction
    {
        public ScheduledAction([NotNull] string name, [NotNull] IScheduler scheduler, [NotNull] ScheduledActionOptions options, [NotNull] Func<IScheduledActionContext, Task> payload)
        {
            Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public IScheduler Scheduler { get; }

        [NotNull]
        public ScheduledActionOptions Options { get; }

        [NotNull]
        public Func<IScheduledActionContext, Task> Payload { get; }
    }
}
