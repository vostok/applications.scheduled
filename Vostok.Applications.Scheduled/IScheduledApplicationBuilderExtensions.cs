using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public static class IScheduledApplicationBuilderExtensions
    {
        public static IScheduledApplicationBuilder Schedule(
            [NotNull] this IScheduledApplicationBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action payload)
            => builder.Schedule(name, scheduler, _ => payload());

        public static IScheduledApplicationBuilder Schedule(
            [NotNull] this IScheduledApplicationBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Func<Task> payload)
            => builder.Schedule(name, scheduler, _ => payload());

        public static IScheduledApplicationBuilder Schedule(
            [NotNull] this IScheduledApplicationBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action payload,
            [NotNull] ScheduledActionOptions options)
            => builder.Schedule(name, scheduler, _ => payload(), options);

        public static IScheduledApplicationBuilder Schedule(
            [NotNull] this IScheduledApplicationBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Func<Task> payload,
            [NotNull] ScheduledActionOptions options)
            => builder.Schedule(name, scheduler, _ => payload(), options);

    }
}
