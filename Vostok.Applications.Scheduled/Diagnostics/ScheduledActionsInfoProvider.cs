using System;
using Vostok.Hosting.Abstractions.Diagnostics;

namespace Vostok.Applications.Scheduled.Diagnostics
{
    internal class ScheduledActionsInfoProvider : IDiagnosticInfoProvider
    {
        private readonly Func<ScheduledActionInfo> infoProvider;

        public ScheduledActionsInfoProvider(Func<ScheduledActionInfo> infoProvider)
            => this.infoProvider = infoProvider;

        public object Query() => infoProvider();
    }
}
