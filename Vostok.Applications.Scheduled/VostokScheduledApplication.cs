using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Helpers;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public abstract class VostokScheduledApplication : IVostokApplication, IDisposable
    {
        private volatile IScheduledActionsRunner runner;

        public async Task InitializeAsync(IVostokHostingEnvironment environment)
            => runner = await ScheduledApplicationHelper.InitializeAsync(environment, (builder, env) => Setup(builder, env));

        public Task RunAsync(IVostokHostingEnvironment environment)
            => runner.RunAsync(environment.ShutdownToken);

        public void Dispose()
        {
            (runner as IDisposable)?.Dispose();
            DoDisposeAsync().GetAwaiter().GetResult();
            DoDispose();
        }

        public abstract void Setup([NotNull] IScheduledActionsBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        public virtual void DoDispose()
        {
        }

        public virtual Task DoDisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}