using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public class ScheduledActionsBuilder : IScheduledActionsBuilder
    {
        private readonly ILog log;
        private readonly ITracer tracer;
        private readonly IVostokApplicationDiagnostics diagnostics;
        private readonly List<ScheduledAction> actions;

        private volatile ScheduledActionsDynamicOptions dynamicOptions;

        public ScheduledActionsBuilder(ILog log)
            : this(log, TracerProvider.Get())
        {
        }

        public ScheduledActionsBuilder(ILog log, ITracer tracer)
            : this(log, tracer, null)
        {
        }

        public ScheduledActionsBuilder(ILog log, ITracer tracer, IVostokApplicationDiagnostics diagnostics)
        {
            this.log = log.ForContext("Scheduler");
            this.tracer = tracer;
            this.diagnostics = diagnostics;

            actions = new List<ScheduledAction>();
        }

        public bool SupportsDynamicConfiguration { get; set; } = true;

        public bool ShouldLogScheduledActions { get; set; } = true;

        public IScheduledActionsRunner BuildRunner()
        {
            if (dynamicOptions != null)
                return new ScheduledActionsDynamicRunner(actions, diagnostics, tracer, log, dynamicOptions);

            return new ScheduledActionsRunner(actions.Select(action => new ScheduledActionRunner(action, log, tracer, diagnostics)).ToArray(), log);
        }

        public IScheduledActionsBuilder Schedule(string name, IScheduler scheduler, Action<IScheduledActionContext> payload)
            => Schedule(name, scheduler, payload, new ScheduledActionOptions());

        public IScheduledActionsBuilder Schedule(string name, IScheduler scheduler, Func<IScheduledActionContext, Task> payload)
            => Schedule(name, scheduler, payload, new ScheduledActionOptions());

        public IScheduledActionsBuilder Schedule(string name, IScheduler scheduler, Action<IScheduledActionContext> payload, ScheduledActionOptions options)
            => Schedule(name, scheduler, WrapAction(payload), options);

        public IScheduledActionsBuilder Schedule(string name, IScheduler scheduler, Func<IScheduledActionContext, Task> payload, ScheduledActionOptions options)
            => Schedule(new ScheduledAction(name, scheduler, options, payload));

        public void SetupDynamic(Action<IScheduledActionsBuilder> configuration, TimeSpan actualizationPeriod)
            => SetupDynamic(WrapConfiguration(configuration), actualizationPeriod);

        public void SetupDynamic(Func<IScheduledActionsBuilder, CancellationToken, Task> configuration, TimeSpan actualizationPeriod)
        {
            if (!SupportsDynamicConfiguration)
                throw new InvalidOperationException("Dynamic configuration mode is not supported by this builder.");

            if (dynamicOptions != null)
                throw new InvalidOperationException("Can't overwrite existing dynamic configuration.");

            dynamicOptions = new ScheduledActionsDynamicOptions(configuration, actualizationPeriod);
        }

        internal IReadOnlyList<ScheduledAction> Actions => actions;

        internal IScheduledActionsBuilder Schedule(ScheduledAction action)
        {
            actions.Add(action);

            if (ShouldLogScheduledActions)
                log.Info("Scheduled '{ActionName}' action with scheduler '{Scheduler}'. ", action.Name, action.Scheduler.GetType().Name);

            return this;
        }

        private static Func<IScheduledActionContext, Task> WrapAction(Action<IScheduledActionContext> action)
        {
            return context =>
            {
                action(context);
                return Task.CompletedTask;
            };
        }

        private static Func<IScheduledActionsBuilder, CancellationToken, Task> WrapConfiguration(Action<IScheduledActionsBuilder> configuration)
        {
            return (builder, cancellationToken) =>
            {
                configuration(builder);
                return Task.CompletedTask;
            };
        }
    }
}