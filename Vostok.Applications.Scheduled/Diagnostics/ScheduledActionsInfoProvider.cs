using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Applications.Scheduled.Diagnostics
{
    internal class ScheduledActionsInfoProvider : IDiagnosticInfoProvider
    {
        private readonly ScheduledActionRunner runner;

        public ScheduledActionsInfoProvider(ScheduledActionRunner runner)
            => this.runner = runner;

        public object Query() => runner.GetInfo();
    }
}
