using System;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    // TODO: Get parameter?
    [PublicAPI]
    public interface IScheduler
    {
        DateTimeOffset? ScheduleNext(DateTimeOffset from);
    }
}
