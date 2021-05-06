using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Applications.Scheduled.Diagnostics;
using Vostok.Applications.Scheduled.Schedulers;
using Vostok.Commons.Time;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Context;
using Vostok.Tracing.Abstractions;
using Vostok.Tracing.Extensions.Custom;

// ReSharper disable MethodSupportsCancellation

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledActionRunner : IDisposable
    {
        private volatile ScheduledAction action;

        private readonly ScheduledActionMonitor monitor = new ScheduledActionMonitor();
        private readonly ILog log;
        private readonly ITracer tracer;

        private volatile IDisposable diagnosticInfoRegistration;
        private volatile IDisposable healthCheckRegistration;

        public ScheduledActionRunner(ScheduledAction action, ILog log, ITracer tracer, IVostokApplicationDiagnostics diagnostics)
        {
            this.action = action;
            this.tracer = tracer;
            this.log = log;

            if (diagnostics != null)
                RegisterDiagnostics(diagnostics);
        }

        public ScheduledActionInfo GetInfo()
            => new ScheduledActionInfo(
                action.Name, 
                action.Scheduler.ToString(), 
                action.Options, 
                monitor.BuildStatistics());

        public void Update(ScheduledAction newAction)
        {
            if (newAction.Name != action.Name)
                throw new InvalidOperationException($"Name mismatch on scheduled action update (old = '{action.Name}', new = '{newAction.Name}').");

            if (newAction.Scheduler is IStatefulScheduler newStateful && action.Scheduler is IStatefulScheduler oldStateful)
                newStateful.TryCopyStateFrom(oldStateful);

            action = newAction;
        }

        public void Dispose()
        {
            diagnosticInfoRegistration?.Dispose();
            healthCheckRegistration?.Dispose();
        }

        public async Task RunAsync(CancellationToken token)
        {
            var lastExecutionTime = PreciseDateTime.Now;
            var iteration = 0L;

            using (new OperationContextToken(action.Name))
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        using (new OperationContextToken($"{++iteration}"))
                        {
                            var scheduler = await WaitForNextExecutionAsync(lastExecutionTime, token);

                            lastExecutionTime = PreciseDateTime.Now;

                            await ExecutePayloadAsync(lastExecutionTime, scheduler, token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }

                log.Info("Finished executing.");
            }
        }

        private async Task<IScheduler> WaitForNextExecutionAsync(DateTimeOffset lastExecutionTime, CancellationToken token)
        {
            var nextExecutionTime = null as DateTimeOffset?;
            var nextExecutionScheduler = null as IScheduler;
            var firstActualizationDone = false;

            while (!token.IsCancellationRequested)
            {
                var (newNextExecutionTime, newNextExecutionScheduler) = GetNextExecutionTime(lastExecutionTime);

                if (newNextExecutionTime != nextExecutionTime || !firstActualizationDone)
                {
                    nextExecutionTime = newNextExecutionTime;
                    nextExecutionScheduler = newNextExecutionScheduler;
                    LogNextExecutionTime(nextExecutionTime);
                    monitor.OnNextExecution(nextExecutionTime);
                }

                firstActualizationDone = true;

                if (nextExecutionTime == null)
                {
                    await Task.Delay(action.Options.ActualizationPeriod, token);
                    continue;
                }

                if (nextExecutionTime <= lastExecutionTime)
                    return nextExecutionScheduler;

                var timeToWait = TimeSpanArithmetics.Max(TimeSpan.Zero, nextExecutionTime.Value - PreciseDateTime.Now);
                if (timeToWait > action.Options.ActualizationPeriod)
                {
                    await Task.Delay(action.Options.ActualizationPeriod, token);
                    continue;
                }

                if (timeToWait > TimeSpan.Zero)
                    await Task.Delay(timeToWait, token);

                while (PreciseDateTime.Now < nextExecutionTime)
                    await Task.Delay(1.Milliseconds(), token);

                return nextExecutionScheduler;
            }

            return null;
        }

        private async Task ExecutePayloadAsync(DateTimeOffset executionTime, IScheduler scheduler, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var nextExecution = GetNextExecutionTime(executionTime).time;

            var timeBudget = nextExecution.HasValue
                ? TimeBudget.StartNew(TimeSpanArithmetics.Max(TimeSpan.Zero, nextExecution.Value - executionTime))
                : TimeBudget.Infinite;

            var context = new ScheduledActionContext(executionTime, timeBudget, scheduler, token);

            if (!(scheduler is PeriodicalWithConstantPauseScheduler))
                log.Info("Executing with time budget = {TimeBudget}.", timeBudget.Total.ToPrettyString());
            else
                log.Info("Executing..");

            async Task ExecutePayload()
            {
                monitor.OnIterationStarted();
                var span = tracer.BeginCustomOperationSpan(action.Name);

                try
                {
                    var watch = Stopwatch.StartNew();

                    await action.Payload(context);

                    watch.Stop();

                    log.Info(
                        "Executed in {ExecutionTime}.",
                        new
                        {
                            ExecutionTime = watch.Elapsed.ToPrettyString(),
                            ExecutionTimeMs = watch.Elapsed.TotalMilliseconds
                        });

                    if (watch.Elapsed > timeBudget.Total && !(scheduler is PeriodicalWithConstantPauseScheduler))
                        log.Warn("Execution did not fit into the time budget before the next planned execution.");

                    action.Scheduler.OnSuccessfulIteration(scheduler);

                    span.SetOperationStatus(null, WellKnownStatuses.Success);
                    monitor.OnIterationSucceeded();
                }
                catch (Exception error)
                {
                    action.Scheduler.OnFailedIteration(scheduler, error);

                    span.SetOperationStatus(null, WellKnownStatuses.Error);
                    monitor.OnIterationFailed(error);

                    if (action.Options.CrashOnPayloadException || error is OperationCanceledException)
                        throw;

                    log.Error(error, "Scheduled action threw an exception.");
                }
                finally
                {
                    span.Dispose();
                    monitor.OnIterationCompleted();
                }
            }

            var payloadTask = action.Options.PreferSeparateThread
                ? Task.Factory.StartNew(ExecutePayload, TaskCreationOptions.LongRunning)
                : Task.Run(ExecutePayload);

            if (action.Options.AllowOverlappingExecution)
                return;

            await payloadTask;
        }

        private (DateTimeOffset? time, IScheduler scheduler) GetNextExecutionTime(DateTimeOffset from)
        {
            try
            {
                return action.Scheduler.ScheduleNextWithSource(from);
            }
            catch (Exception error)
            {
                if (action.Options.CrashOnSchedulerException)
                    throw;

                log.Error(error, "Scheduler failure. Can't schedule next iteration.");

                return (null, null);
            }
        }

        private void LogNextExecutionTime(DateTimeOffset? nextExecutionTime)
        {
            if (nextExecutionTime == null)
                log.Info("Next execution time: unknown.");
            else
                log.Info("Next execution time = {NextExecutionTime:yyyy-MM-dd HH:mm:ss.fff} (~{TimeToNextExecution} from now).", 
                    nextExecutionTime.Value.DateTime, TimeSpanArithmetics.Max(TimeSpan.Zero, nextExecutionTime.Value - PreciseDateTime.Now).ToPrettyString());
        }

        private void RegisterDiagnostics(IVostokApplicationDiagnostics diagnostics)
        {
            var info = GetInfo();
            var infoEntry = new DiagnosticEntry("scheduled", info.Name);
            var infoProvider = new ScheduledActionsInfoProvider(GetInfo);
            var healthCheck = new ScheduledActionsHealthCheck(GetInfo);

            diagnosticInfoRegistration = diagnostics.Info.RegisterProvider(infoEntry, infoProvider);
            healthCheckRegistration = diagnostics.HealthTracker.RegisterCheck($"scheduled ({info.Name})", healthCheck);
        }
    }
}
