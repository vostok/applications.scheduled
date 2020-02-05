using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public static class IScheduledActionsBuilderExtensions
    {
        public static IScheduledActionsBuilder Schedule(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action payload)
            => builder.Schedule(name, scheduler, _ => payload());

        public static IScheduledActionsBuilder Schedule(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Func<Task> payload)
            => builder.Schedule(name, scheduler, _ => payload());

        public static IScheduledActionsBuilder Schedule(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action payload,
            [NotNull] ScheduledActionOptions options)
            => builder.Schedule(name, scheduler, _ => payload(), options);

        public static IScheduledActionsBuilder Schedule(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Func<Task> payload,
            [NotNull] ScheduledActionOptions options)
            => builder.Schedule(name, scheduler, _ => payload(), options);

    }
}
