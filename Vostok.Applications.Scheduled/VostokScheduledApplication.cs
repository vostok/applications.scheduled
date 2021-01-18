using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public abstract class VostokScheduledApplication : VostokScheduledAsyncApplication
    {
        public abstract void Setup([NotNull] IScheduledActionsBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        public override Task SetupAsync(IScheduledActionsBuilder builder, IVostokHostingEnvironment environment)
        {
            Setup(builder, environment);
            
            return Task.CompletedTask;
        }
    }
}
