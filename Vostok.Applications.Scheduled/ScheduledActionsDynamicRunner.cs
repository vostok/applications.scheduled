using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Helpers.Extensions;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

// ReSharper disable MethodSupportsCancellation

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledActionsDynamicRunner : IScheduledActionsRunner
    {
        private readonly IVostokApplicationDiagnostics diagnostics;
        private readonly ITracer tracer;
        private readonly ILog log;
        private readonly ScheduledActionRunner actualizationRunner;
        private readonly ConcurrentDictionary<string, UserActionRunner> userRunners;

        public ScheduledActionsDynamicRunner(IVostokApplicationDiagnostics diagnostics, ITracer tracer, ILog log, ScheduledActionsDynamicOptions options)
        {
            this.diagnostics = diagnostics;
            this.tracer = tracer;
            this.log = log;
            
            actualizationRunner = new ScheduledActionRunner(ConfigureActualizationAction(options), log, tracer, diagnostics);
            userRunners = new ConcurrentDictionary<string, UserActionRunner>();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await actualizationRunner.RunAsync(cancellationToken).SilentlyContinue();

            await Task.WhenAll(userRunners.Select(pair => pair.Value.WaitForCompletion()));
        }

        public void Dispose()
            => actualizationRunner.Dispose();

        private ScheduledAction ConfigureActualizationAction(ScheduledActionsDynamicOptions options)
        {
            var scheduler = Scheduler.Periodical(options.ActualizationPeriod, false);

            var actionOptions = new ScheduledActionOptions();

            return new ScheduledAction(
                "ActualizeScheduledActionsSet",
                scheduler,
                actionOptions,
                async context =>
                {
                    var builder = new ScheduledActionsBuilder(log, tracer, diagnostics)
                    {
                        SupportsDynamicConfiguration = false
                    };

                    await options.Configuration(builder, context.CancellationToken);

                    Actualize(builder.Actions, context.CancellationToken);
                });
        }

        private void Actualize(List<ScheduledAction> actualActions, CancellationToken cancellation)
        {
            var actualIndex = new HashSet<string>(actualActions.Select(action => action.Name));

            foreach (var pair in userRunners)
            {
                if (!actualIndex.Contains(pair.Key))
                    pair.Value.RequestShutdown();
            }

            foreach (var action in actualActions)
            {
                var runner = userRunners.GetOrAdd(action.Name, _ => LaunchUserActionRunner(action, cancellation));
                if (runner.CanBeUpdated)
                    runner.Update(action);
            }
        }

        private UserActionRunner LaunchUserActionRunner(ScheduledAction action, CancellationToken cancellationToken)
        {
            var runner = new ScheduledActionRunner(action, log, tracer, diagnostics);

            var personalCancellationSource = new CancellationTokenSource();

            var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, personalCancellationSource.Token);

            var runnerTask = runner.RunAsync(linkedCancellationSource.Token).ContinueWith(task =>
            {
                userRunners.TryRemove(action.Name, out _);

                runner.Dispose();

                linkedCancellationSource.Dispose();

                personalCancellationSource.Dispose();
            });

            return new UserActionRunner(personalCancellationSource, runner, runnerTask);
        }

        private class UserActionRunner
        {
            private readonly CancellationTokenSource cancellation;
            private readonly ScheduledActionRunner runner;
            private readonly Task runnerTask;

            public UserActionRunner(CancellationTokenSource cancellation, ScheduledActionRunner runner, Task runnerTask)
            {
                this.cancellation = cancellation;
                this.runner = runner;
                this.runnerTask = runnerTask;
            }

            public bool CanBeUpdated
                => !runnerTask.IsCompleted && !cancellation.IsCancellationRequested;

            public void Update(ScheduledAction action) 
                => runner.Update(action);

            public Task WaitForCompletion() 
                => runnerTask;

            public void RequestShutdown()
            {
                try
                {
                    cancellation.Cancel();
                }
                catch
                {
                     // ignored (potential harmless race with CTS disposal)
                }
            }
        }
    }
}