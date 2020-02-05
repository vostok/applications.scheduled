using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public abstract class VostokScheduledApplication : IVostokApplication
    {
        private volatile IScheduledActionsRunner runner;

        public abstract void Setup([NotNull] IScheduledActionsBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        public Task InitializeAsync(IVostokHostingEnvironment environment)
        {
            var builder = new ScheduledActionsBuilder(environment.Log);

            Setup(builder, environment);

            runner = builder.BuildRunner();

            return Task.CompletedTask;
        }

        public Task RunAsync(IVostokHostingEnvironment environment)
            => runner.RunAsync(environment.ShutdownToken);
    }
}
