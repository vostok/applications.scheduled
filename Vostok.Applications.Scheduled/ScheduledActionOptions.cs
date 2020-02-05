using System;
using JetBrains.Annotations;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public class ScheduledActionOptions
    {
        public bool CrashOnPayloadException { get; set; }
        
        public bool CrashOnSchedulerException { get; set; }
     
        public bool PreferSeparateThread { get; set; }

        public bool AllowOverlappingExecution { get; set; }

        public TimeSpan ActualizationPeriod { get; set; } = 1.Seconds();
    }
}
