using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Diagnostics;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public abstract class VostokScheduledApplication : IVostokApplication, IDisposable
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        private volatile ScheduledActionsRunner runner;

        public abstract void Setup([NotNull] IScheduledActionsBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        public async Task InitializeAsync(IVostokHostingEnvironment environment)
        {
            var builder = new ScheduledActionsBuilder(environment.Log);

            await WarmupBeforeSetupAsync(environment);

            Setup(builder, environment);

            await WarmupAfterSetupAsync(environment);

            runner = builder.BuildRunnerInternal();

            RegisterDiagnosticFeatures(builder, environment);
        }

        public Task RunAsync(IVostokHostingEnvironment environment)
            => runner.RunAsync(environment.ShutdownToken);

        public void Dispose()
            => disposables.ForEach(disposable => disposable.Dispose());

        public virtual Task WarmupBeforeSetupAsync([NotNull] IVostokHostingEnvironment environment) 
            => Task.CompletedTask;

        public virtual Task WarmupAfterSetupAsync([NotNull] IVostokHostingEnvironment environment)
            => Task.CompletedTask;

        private void RegisterDiagnosticFeatures(ScheduledActionsBuilder builder, IVostokHostingEnvironment environment)
        {
            foreach (var actionRunner in runner.Runners)
            {
                var info = actionRunner.GetInfo();
                var infoEntry = new DiagnosticEntry("scheduled", info.Name);
                var infoProvider = new ScheduledActionsInfoProvider(actionRunner);
                var healthCheck = new ScheduledActionsHealthCheck(actionRunner);

                if (builder.DiagnosticInfoEnabled)
                    disposables.Add(environment.Diagnostics.Info.RegisterProvider(infoEntry, infoProvider));
                
                if (builder.DiagnosticChecksEnabled)
                    disposables.Add(environment.Diagnostics.HealthTracker.RegisterCheck($"scheduled ({info.Name})", healthCheck));
            }
        }
    }
}
