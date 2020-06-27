using System;
using JetBrains.Annotations;
using NCrontab;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class CrontabScheduler : IScheduler
    {
        private readonly CrontabSchedule.ParseOptions OptionsWithSeconds = new CrontabSchedule.ParseOptions {IncludingSeconds = true};
        private readonly CrontabSchedule.ParseOptions OptionsWithoutSeconds = new CrontabSchedule.ParseOptions {IncludingSeconds = false};

        private readonly Func<string> scheduleProvider;

        public CrontabScheduler([NotNull] Func<string> scheduleProvider)
            => this.scheduleProvider = scheduleProvider ?? throw new ArgumentNullException(nameof(scheduleProvider));

        public DateTimeOffset? ScheduleNext(DateTimeOffset from)
        {
            var schedule = ParseSchedule(scheduleProvider());
            var nextOccurence = schedule.GetNextOccurrence(from.DateTime);
            if (nextOccurence == DateTime.MaxValue)
                return null;

            return nextOccurence;
        }

        public override string ToString() => $"Crontab({scheduleProvider()})";

        private CrontabSchedule ParseSchedule(string value)
        {
            var schedule = CrontabSchedule.TryParse(value, OptionsWithSeconds) ?? CrontabSchedule.Parse(value, OptionsWithoutSeconds);
            if (schedule == null)
                throw new FormatException($"Failed to parse value '{value}' as a crontab schedule.");

            return schedule;
        }
    }
}
