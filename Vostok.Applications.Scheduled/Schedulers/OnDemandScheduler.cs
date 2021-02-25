using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Signal = System.Threading.Tasks.TaskCompletionSource<bool>;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class OnDemandScheduler : IScheduler, IScheduledActionEventListener
    {
        protected readonly object Sync = new object();
        private readonly List<Signal> signals = new List<Signal>();
        private volatile bool activated;

        public void Demand()
        {
            lock (Sync)
                activated = true;
        }

        public Task DemandWithFeedback()
        {
            var signal = new Signal(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (Sync)
            {
                signals.Add(signal);
                activated = true;
            }

            return signal.Task;
        }

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            lock (Sync)
            {
                if (!activated)
                    return null;

                activated = false;

                return from;
            }
        }

        public void OnSuccessfulIteration(IScheduler source)
        {
            lock (Sync)
            {
                signals.ForEach(signal => signal.TrySetResult(true));
                signals.Clear();
            }
        }

        public void OnFailedIteration(IScheduler source, Exception error)
        {
            lock (Sync)
            {
                signals.ForEach(signal => signal.TrySetException(error));
                signals.Clear();
            }
        }

        public override string ToString() => "OnDemand";
    }

    internal class OnDemandSchedulerWithArgument<TArg> : OnDemandScheduler, IScheduler<TArg>
    {
        private TArg lastArgument;
        
        public void Demand(TArg argument)
        {
            lock (Sync)
            {
                lastArgument = argument;
                base.Demand();
            }
        }

        public Task DemandWithFeedback(TArg argument)
        {
            lock (Sync)
            {
                lastArgument = argument;
                return base.DemandWithFeedback();
            }
        }

        public TArg GetLastArgument() => lastArgument;
    }
}
