using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Applications.Scheduled.Diagnostics;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Applications.Scheduled.Helpers
{
    internal static class ScheduledApplicationHelper
    {
        public static async Task<(ScheduledActionsRunner, List<IDisposable>)> InitializeAsync(
            IVostokHostingEnvironment environment,
            Func<IScheduledActionsBuilder, IVostokHostingEnvironment, Task> setupRunner)
        {
            var builder = new ScheduledActionsBuilder(environment.Log, environment.Tracer);

            await setupRunner(builder, environment).ConfigureAwait(false);

            var runner = builder.BuildRunnerInternal();

            return (runner, RegisterDiagnosticFeatures(environment, runner));
        }

        private static List<IDisposable> RegisterDiagnosticFeatures(IVostokHostingEnvironment environment, ScheduledActionsRunner runner)
        {
            var disposables = new List<IDisposable>();

            if (!environment.HostExtensions.TryGet<IVostokApplicationDiagnostics>(out var diagnostics))
                return disposables;

            foreach (var actionRunner in runner.Runners)
            {
                var info = actionRunner.GetInfo();
                var infoEntry = new DiagnosticEntry("scheduled", info.Name);
                var infoProvider = new ScheduledActionsInfoProvider(actionRunner);
                var healthCheck = new ScheduledActionsHealthCheck(actionRunner);

                disposables.Add(diagnostics.Info.RegisterProvider(infoEntry, infoProvider));
                disposables.Add(diagnostics.HealthTracker.RegisterCheck($"scheduled ({info.Name})", healthCheck));
            }

            return disposables;
        }
    }
}