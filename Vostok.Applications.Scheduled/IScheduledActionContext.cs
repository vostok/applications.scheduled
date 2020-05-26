using System;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public interface IScheduledActionContext
    {
        /// <summary>
        /// <para>Returns the scheduler instance that caused this particular iteration.</para>
        /// <para>If using a multi scheduler, its relevant component is returned.</para>
        /// </summary>
        IScheduler Scheduler { get; }

        /// <summary>
        /// Returns the time remaining until the next iteration.
        /// </summary>
        TimeSpan RemainingTime { get; }

        /// <summary>
        /// Returns the time when execution begins.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        CancellationToken CancellationToken { get; }
    }
}
