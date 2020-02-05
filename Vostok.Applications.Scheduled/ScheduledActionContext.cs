using System;
using System.Threading;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledActionContext : IScheduledActionContext
    {
        private readonly TimeBudget budget;

        public ScheduledActionContext(TimeBudget budget, CancellationToken cancellationToken)
        {
            this.budget = budget;
            CancellationToken = cancellationToken;
        }

        public TimeSpan RemainingTime => budget.Remaining;

        public CancellationToken CancellationToken { get; }
    }
}
