using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Helpers;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public abstract class VostokScheduledAsyncApplication : IVostokApplication, IDisposable
    {
        private List<IDisposable> disposables;

        private volatile ScheduledActionsRunner runner;

        public async Task InitializeAsync(IVostokHostingEnvironment environment)
        {
            (runner, disposables) = await ScheduledApplicationHelper.InitializeAsync(environment, SetupAsync).ConfigureAwait(false);
        }

        public Task RunAsync(IVostokHostingEnvironment environment)
            => runner.RunAsync(environment.ShutdownToken);

        public void Dispose()
        {
            disposables.ForEach(disposable => disposable.Dispose());
            DoDisposeAsync().GetAwaiter().GetResult();
        }

        protected abstract Task SetupAsync([NotNull] IScheduledActionsBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        protected virtual Task DoDisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}