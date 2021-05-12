using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Helpers;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public abstract class VostokScheduledAsyncApplication : IVostokApplication, IDisposable
    {
        private volatile IScheduledActionsRunner runner;

        public async Task InitializeAsync(IVostokHostingEnvironment environment)
            => runner = await ScheduledApplicationHelper.InitializeAsync(environment, SetupAsync);

        public Task RunAsync(IVostokHostingEnvironment environment)
            => runner.RunAsync(environment.ShutdownToken);

        public void Dispose()
        {
            (runner as IDisposable)?.Dispose();
            DoDisposeAsync().GetAwaiter().GetResult();
        }

        protected abstract Task SetupAsync([NotNull] IScheduledActionsBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        protected virtual Task DoDisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}