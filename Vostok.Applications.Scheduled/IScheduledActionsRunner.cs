using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public interface IScheduledActionsRunner
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
