using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Applications.Scheduled.Diagnostics
{
    internal class ScheduledActionsHealthCheck : IHealthCheck
    {
        private readonly Func<ScheduledActionInfo> infoProvider;

        public ScheduledActionsHealthCheck(Func<ScheduledActionInfo> infoProvider)
            => this.infoProvider = infoProvider;

        public Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
        {
            var info = infoProvider();

            if (!info.Statistics.LastIterationSuccessful)
                return Task.FromResult(HealthCheckResult.Degraded($"Scheduled action '{info.Name}' has failed on its last execution with error '{info.Statistics.LastErrorMessage}'."));

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
