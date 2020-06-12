// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Vostok.Applications.Scheduled.Diagnostics
{
    internal class ScheduledActionInfo
    {
        public ScheduledActionInfo(
            string name, 
            string scheduler, 
            ScheduledActionOptions options, 
            ScheduledActionStatistics statistics)
        {
            Name = name;
            Scheduler = scheduler;
            Options = options;
            Statistics = statistics;
        }

        public string Name { get; }

        public string Scheduler { get; }

        public ScheduledActionOptions Options { get; }

        public ScheduledActionStatistics Statistics { get; }
    }
}
