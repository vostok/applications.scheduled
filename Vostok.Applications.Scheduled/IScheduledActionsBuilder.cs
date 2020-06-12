using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public interface IScheduledActionsBuilder
    {
        IScheduledActionsBuilder Schedule(
            [NotNull] string name, 
            [NotNull] IScheduler scheduler, 
            [NotNull] Action<IScheduledActionContext> payload);

        IScheduledActionsBuilder Schedule(
            [NotNull] string name, 
            [NotNull] IScheduler scheduler, 
            [NotNull] Func<IScheduledActionContext, Task> payload);

        IScheduledActionsBuilder Schedule(
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action<IScheduledActionContext> payload,
            [NotNull] ScheduledActionOptions options);

        IScheduledActionsBuilder Schedule(
            [NotNull] string name, 
            [NotNull] IScheduler scheduler, 
            [NotNull] Func<IScheduledActionContext, Task> payload, 
            [NotNull] ScheduledActionOptions options);

        IScheduledActionsBuilder DisableDiagnosticInfo();

        IScheduledActionsBuilder DisableDiagnosticChecks();
    }
}
