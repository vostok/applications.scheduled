using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Signal = System.Threading.Tasks.TaskCompletionSource<bool>;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class OnDemandScheduler : IScheduler, IScheduledActionEventListener
    {
        private readonly object sync = new object();
        private readonly List<Signal> signals = new List<Signal>();
        private volatile bool activated;

        public void Demand()
        {
            lock (sync)
                activated = true;
        }

        public Task DemandWithFeedback()
        {
            var signal = new Signal(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (sync)
            {
                signals.Add(signal);
                activated = true;
            }

            return signal.Task;
        }

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            lock (sync)
            {
                if (!activated)
                    return null;

                activated = false;

                return from;
            }
        }

        public void OnSuccessfulIteration(IScheduler source)
        {
            lock (sync)
            {
                signals.ForEach(signal => signal.TrySetResult(true));
                signals.Clear();
            }
        }

        public void OnFailedIteration(IScheduler source, Exception error)
        {
            lock (sync)
            {
                signals.ForEach(signal => signal.TrySetException(error));
                signals.Clear();
            }
        }

        public override string ToString() => "OnDemand";
    }
}
