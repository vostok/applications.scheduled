using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Schedulers;

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

        public static IScheduledActionsBuilder Schedule<TArg>(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action<TArg> payload)
            => builder.Schedule(name, scheduler, _ => payload(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler)));

        public static IScheduledActionsBuilder Schedule<TArg>(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Func<TArg, Task> payload)
            => builder.Schedule(name, scheduler, _ => payload(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler)));

        public static IScheduledActionsBuilder Schedule<TArg>(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action<TArg> payload,
            [NotNull] ScheduledActionOptions options)
            => builder.Schedule(name, scheduler, _ => payload(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler)), options);

        public static IScheduledActionsBuilder Schedule<TArg>(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Func<TArg, Task> payload,
            [NotNull] ScheduledActionOptions options) 
            => builder.Schedule(name, scheduler, _ => payload(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler)), options);

        public static IScheduledActionsBuilder Schedule<TArg>(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action<TArg, IScheduledActionContext> payload)
            => builder.Schedule(name, scheduler, context => payload(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler), context));

        public static IScheduledActionsBuilder Schedule<TArg>(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Func<TArg, IScheduledActionContext, Task> payload)
            => builder.Schedule(name, scheduler, context => payload(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler), context));

        public static IScheduledActionsBuilder Schedule<TArg>(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Action<TArg, IScheduledActionContext> payload,
            [NotNull] ScheduledActionOptions options)
            => builder.Schedule(name, scheduler, context => payload(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler), context), options);

        public static IScheduledActionsBuilder Schedule<TArg>(
            [NotNull] this IScheduledActionsBuilder builder,
            [NotNull] string name,
            [NotNull] IScheduler scheduler,
            [NotNull] Func<TArg, IScheduledActionContext, Task> payload,
            [NotNull] ScheduledActionOptions options) 
            => builder.Schedule(name, scheduler, context => payload(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler), context), options);

        private static TArg ExtractArgumentFromOnDemandScheduler<TArg>(IScheduler scheduler)
        {
            if (scheduler is OnDemandSchedulerWithArgument<TArg> onDemandScheduler)
                return onDemandScheduler.GetLastArgumentValue();

            throw new NotSupportedException("Argument passing is supported for OnDemandScheduler only.");
        }
    }
}