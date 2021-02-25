using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Schedulers;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public class ScheduledActionsBuilder : IScheduledActionsBuilder
    {
        private readonly ILog log;
        private readonly ITracer tracer;
        private readonly List<ScheduledAction> actions;

        public ScheduledActionsBuilder(ILog log)
            : this(log, TracerProvider.Get())
        {
        }

        public ScheduledActionsBuilder(ILog log, ITracer tracer)
        {
            this.log = log.ForContext("Scheduler");
            this.tracer = tracer;

            actions = new List<ScheduledAction>();
        }

        public IScheduledActionsRunner BuildRunner()
            => BuildRunnerInternal();

        public IScheduledActionsBuilder Schedule(string name, IScheduler scheduler, Action<IScheduledActionContext> payload)
            => Schedule(name, scheduler, payload, new ScheduledActionOptions());

        public IScheduledActionsBuilder Schedule(string name, IScheduler scheduler, Func<IScheduledActionContext, Task> payload)
            => Schedule(name, scheduler, payload, new ScheduledActionOptions());

        public IScheduledActionsBuilder Schedule(string name, IScheduler scheduler, Action<IScheduledActionContext> payload, ScheduledActionOptions options)
            => Schedule(name, scheduler, WrapAction(payload), options);

        public IScheduledActionsBuilder Schedule(string name, IScheduler scheduler, Func<IScheduledActionContext, Task> payload, ScheduledActionOptions options)
        {
            actions.Add(new ScheduledAction(name, scheduler, options, payload));

            log.Info("Scheduled '{ActionName}' action with scheduler '{Scheduler}'. ", name, scheduler.GetType().Name);

            return this;
        }

        public IScheduledActionsBuilder Schedule<TArg>(string name, IScheduler scheduler, Func<TArg, IScheduledActionContext, Task> payload, ScheduledActionOptions options) =>
            Schedule(name, scheduler, WrapArgumentExtraction(payload, scheduler), options);

        internal ScheduledActionsRunner BuildRunnerInternal()
            => new ScheduledActionsRunner(actions.Select(action => new ScheduledActionRunner(action, log, tracer)).ToArray(), log);

        private static Func<IScheduledActionContext, Task> WrapAction(Action<IScheduledActionContext> action)
        {
            return context =>
            {
                action(context);
                return Task.CompletedTask;
            };
        }

        private static TArg ExtractArgumentFromOnDemandScheduler<TArg>(IScheduler scheduler)
        {
            if (scheduler is OnDemandSchedulerWithArgument<TArg> onDemandScheduler)
                return onDemandScheduler.GetLastArgument();

            throw new NotSupportedException("Argument passing is supported for OnDemandScheduler only.");
        }

        private static Func<IScheduledActionContext, Task> WrapArgumentExtraction<TArg>(Func<TArg, IScheduledActionContext, Task> action, IScheduler scheduler)
        {
            return context => action(ExtractArgumentFromOnDemandScheduler<TArg>(scheduler), context);
        }
    }
}
