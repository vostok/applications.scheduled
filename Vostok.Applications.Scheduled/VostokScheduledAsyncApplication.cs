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
    public abstract class VostokScheduledAsyncApplication : IVostokApplication, IDisposable
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        private volatile ScheduledActionsRunner runner;

        public abstract Task SetupAsync([NotNull] IScheduledActionsBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        public async Task InitializeAsync(IVostokHostingEnvironment environment)
        {
            var builder = new ScheduledActionsBuilder(environment.Log);

            await SetupAsync(builder, environment).ConfigureAwait(false);

            runner = builder.BuildRunnerInternal();

            RegisterDiagnosticFeatures(environment);
        }

        public Task RunAsync(IVostokHostingEnvironment environment)
            => runner.RunAsync(environment.ShutdownToken);

        public void Dispose()
        {
            disposables.ForEach(disposable => disposable.Dispose());
            DoDisposeAsync().GetAwaiter().GetResult();
            DoDispose();
        }

        public virtual void DoDispose()
        {
        }

        public virtual Task DoDisposeAsync()
        {
            return Task.CompletedTask;
        }

        private void RegisterDiagnosticFeatures(IVostokHostingEnvironment environment)
        {
            if (!environment.HostExtensions.TryGet<IVostokApplicationDiagnostics>(out var diagnostics))
                return;

            foreach (var actionRunner in runner.Runners)
            {
                var info = actionRunner.GetInfo();
                var infoEntry = new DiagnosticEntry("scheduled", info.Name);
                var infoProvider = new ScheduledActionsInfoProvider(actionRunner);
                var healthCheck = new ScheduledActionsHealthCheck(actionRunner);

                disposables.Add(diagnostics.Info.RegisterProvider(infoEntry, infoProvider));
                disposables.Add(diagnostics.HealthTracker.RegisterCheck($"scheduled ({info.Name})", healthCheck));
            }
        }
    }
}