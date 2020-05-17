using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Schedulers;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public static class IScheduledActionContextExtensions
    {
        public static bool IsOnDemandIteration([NotNull] this IScheduledActionContext context)
            => context.Scheduler is OnDemandScheduler;
    }
}
