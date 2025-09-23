using System;
using JetBrains.Annotations;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public class ScheduledActionOptions
    {
        public bool CrashOnPayloadException { get; set; }

        public bool CrashOnPayloadOutOfMemoryException { get; set; } = true;

        public bool CrashOnSchedulerException { get; set; }

        public bool CrashOnSchedulerOutOfMemoryException { get; set; } = true;

        public bool PreferSeparateThread { get; set; }

        /// <summary>
        /// Warning: using with <see cref="PeriodicalWithConstantPauseScheduler"/> may lead to unexpected consequences.
        /// </summary>
        public bool AllowOverlappingExecution { get; set; }

        public TimeSpan ActualizationPeriod { get; set; } = 1.Seconds();
    }
}