using System;
using System.Threading.Tasks;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class OnDemandScheduler : IScheduler
    {
        private volatile TaskCompletionSource<bool> signal = new TaskCompletionSource<bool>(false, TaskCreationOptions.RunContinuationsAsynchronously);

        public void Demand()
            => signal.TrySetResult(true);

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            if (!signal.Task.IsCompleted)
                return null;

            signal = new TaskCompletionSource<bool>(false);

            return from;
        }
    }
}
