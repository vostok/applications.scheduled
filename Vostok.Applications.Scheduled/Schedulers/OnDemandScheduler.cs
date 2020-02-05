using System;
using Vostok.Commons.Threading;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class OnDemandScheduler : IScheduler
    {
        private volatile AtomicBoolean signal = new AtomicBoolean(false);

        public void Demand()
            => signal.TrySetTrue();

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            if (!signal)
                return null;

            signal = new AtomicBoolean(false);

            return from;
        }
    }
}
