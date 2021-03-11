using System;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public interface IScheduler
    {
        DateTimeOffset? ScheduleNext(DateTimeOffset from);
    }
}
