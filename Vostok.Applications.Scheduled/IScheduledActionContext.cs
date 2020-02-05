using System;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public interface IScheduledActionContext
    {
        TimeSpan RemainingTime { get; }

        CancellationToken CancellationToken { get; }
    }
}
