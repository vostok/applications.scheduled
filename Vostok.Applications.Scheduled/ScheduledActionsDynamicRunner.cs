using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly ConcurrentDictionary<string, (ScheduledActionRunner runner, Task task)> userRunners;
        private volatile TaskCompletionSource<List<ScheduledAction>> actualizationSignal;

        public ScheduledActionsDynamicRunner(IVostokApplicationDiagnostics diagnostics, ITracer tracer, ILog log, ScheduledActionsDynamicOptions options)
        {
            this.diagnostics = diagnostics;
            this.tracer = tracer;
            this.log = log;
            
            actualizationSignal = new TaskCompletionSource<List<ScheduledAction>>(TaskCreationOptions.RunContinuationsAsynchronously);
            actualizationRunner = new ScheduledActionRunner(ConfigureActualizationAction(options), log, tracer, diagnostics);

            userRunners = new ConcurrentDictionary<string, (ScheduledActionRunner runner, Task task)>();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var cancellationTask = cancellationToken.WaitAsync();
            var actualizerTask = actualizationRunner.RunAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var completedTask = await Task.WhenAny(actualizationSignal.Task, cancellationTask);

                if (completedTask == cancellationTask)
                {
                    // todo (iloktionov, 06.05.2021): break out
                }

                if (completedTask == actualizationSignal.Task)
                {
                    // todo (iloktionov, 06.05.2021): actualize
                }

                // todo (iloktionov, 06.05.2021): handle user runner crash
            }

            // todo (iloktionov, 06.05.2021): wait for everything to complete
        }

        public void Dispose()
        {
            actualizationRunner.Dispose();

            throw new NotImplementedException();
        }

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
                    if (actualizationSignal.Task.IsCompleted)
                        return;

                    var builder = new ScheduledActionsBuilder(log, tracer, diagnostics);

                    await options.Setup(builder, context.CancellationToken).ConfigureAwait(false);

                    actualizationSignal.TrySetResult(builder.Actions);
                });
        }

        // todo (iloktionov, 06.05.2021): local cancellation
        private (ScheduledActionRunner runner, Task task) LaunchRunner(ScheduledAction action, CancellationToken cancellation)
        {
            var runner = new ScheduledActionRunner(action, log, tracer, diagnostics);

            var runnerTask = runner.RunAsync(cancellation).ContinueWith(task =>
            {
                userRunners.TryRemove(action.Name, out _);

                runner.Dispose();
            });

            return (runner, runnerTask);
        }
    }
}