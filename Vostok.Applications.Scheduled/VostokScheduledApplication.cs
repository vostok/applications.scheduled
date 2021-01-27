using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public abstract class VostokScheduledApplication : VostokScheduledAsyncApplication
    {
        public abstract void Setup([NotNull] IScheduledActionsBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        public sealed override Task SetupAsync(IScheduledActionsBuilder builder, IVostokHostingEnvironment environment)
        {
            Setup(builder, environment);

            return Task.CompletedTask;
        }

        public virtual void DoDispose()
        {
        }

        public virtual Task DoDisposeAsync()
        {
            return Task.CompletedTask;
        }

        protected sealed override async Task DisposeAsync()
        {
            await DoDisposeAsync().ConfigureAwait(false);

            // ReSharper disable once MethodHasAsyncOverload
            DoDispose();
        }
    }
}