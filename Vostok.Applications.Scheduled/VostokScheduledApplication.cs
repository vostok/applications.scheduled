using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Applications.Scheduled.Helpers;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public abstract class VostokScheduledApplication : IVostokApplication, IDisposable
    {
        private List<IDisposable> disposables;

        private volatile ScheduledActionsRunner runner;

        public async Task InitializeAsync(IVostokHostingEnvironment environment)
        {
            (runner, disposables) = await ScheduledApplicationHelper.InitializeAsync(
                    environment,
                    (builder, env) =>
                    {
                        Setup(builder, env);
                        return Task.CompletedTask;
                    })
                .ConfigureAwait(false);
        }

        public Task RunAsync(IVostokHostingEnvironment environment)
            => runner.RunAsync(environment.ShutdownToken);

        public void Dispose()
        {
            disposables?.ForEach(disposable => disposable.Dispose());
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