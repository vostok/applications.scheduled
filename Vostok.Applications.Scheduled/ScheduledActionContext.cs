using System;
using System.Threading;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledActionContext : IScheduledActionContext
    {
        private readonly TimeBudget budget;

        public ScheduledActionContext(
            DateTimeOffset timestamp,
            TimeBudget budget,
            IScheduler scheduler,
            CancellationToken cancellationToken)
        {
            Timestamp = timestamp;
            this.budget = budget;
            Scheduler = scheduler;
            CancellationToken = cancellationToken;
        }

        public DateTimeOffset Timestamp { get; }

        public TimeSpan RemainingTime => budget.Remaining;

        public CancellationToken CancellationToken { get; }

        public IScheduler Scheduler { get; }
    }
}