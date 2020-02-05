using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public interface IScheduledApplicationBuilder
    {
        IScheduledApplicationBuilder Schedule(
            [NotNull] string name, 
            [NotNull] IScheduler scheduler, 
            [NotNull] Action<IScheduledActionContext> payload);

        IScheduledApplicationBuilder Schedule(
            [NotNull] string name, 
            [NotNull] IScheduler scheduler, 
            [NotNull] Func<IScheduledActionContext, Task> payload);

        IScheduledApplicationBuilder Schedule(
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action<IScheduledActionContext> payload,
            [NotNull] ScheduledActionOptions options);

        IScheduledApplicationBuilder Schedule(
            [NotNull] string name, 
            [NotNull] IScheduler scheduler, 
            [NotNull] Func<IScheduledActionContext, Task> payload, 
            [NotNull] ScheduledActionOptions options);
    }
}
