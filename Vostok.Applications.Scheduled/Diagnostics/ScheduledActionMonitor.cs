using System;
using Vostok.Commons.Time;

// ReSharper disable PossibleInvalidOperationException

namespace Vostok.Applications.Scheduled.Diagnostics
{
    internal class ScheduledActionMonitor
    {
        private readonly object sync = new object();

        private volatile bool currentlyExecuting;
        private volatile bool lastIterationSuccessful = true;
        private DateTimeOffset? nextExecution;
        private DateTimeOffset? lastStart;
        private DateTimeOffset? lastFinish;
        private DateTimeOffset? lastError;
        private string lastErrorMessage;
        private TimeSpan lastDuration;
        private TimeSpan totalDuration;
        private TimeSpan averageDuration;
        private int iterationsStarted;
        private int iterationsFailed;
        private int iterationsSucceeded;
        private int iterationsCompleted;

        public void OnNextExecution(DateTimeOffset? timestamp)
        {
            lock (sync)
                nextExecution = timestamp;
        }

        public void OnIterationStarted()
        {
            lock (sync)
            {
                currentlyExecuting = true;
                iterationsStarted++;
                lastStart = PreciseDateTime.Now;
            }
        }

        public void OnIterationFailed(Exception error)
        {
            lock (sync)
            {
                iterationsFailed++;
                lastError = PreciseDateTime.Now;
                lastErrorMessage = $"{error.GetType().Name}: {error.Message}";
                lastIterationSuccessful = false;
            }
        }

        public void OnIterationSucceeded()
        {
            lock (sync)
            {
                iterationsSucceeded++;
                lastIterationSuccessful = true;
            }
        }

        public void OnIterationCompleted()
        {
            lock (sync)
            {
                currentlyExecuting = false;
                iterationsCompleted++;
                lastFinish = PreciseDateTime.Now;
                lastDuration = lastFinish.Value - lastStart.Value;
                totalDuration += lastDuration;
                averageDuration = totalDuration.Divide(iterationsCompleted);
            }
        }

        public ScheduledActionStatistics BuildStatistics()
        {
            lock (sync)
                return new ScheduledActionStatistics(
                    currentlyExecuting,
                    currentlyExecuting ? PreciseDateTime.Now - lastStart.Value : TimeSpan.Zero,
                    nextExecution.HasValue ? TimeSpanArithmetics.Max(nextExecution.Value - PreciseDateTime.Now, TimeSpan.Zero) : null as TimeSpan?,
                    nextExecution,
                    lastStart,
                    lastFinish,
                    lastError,
                    lastErrorMessage,
                    lastIterationSuccessful,
                    lastDuration,
                    totalDuration,
                    averageDuration,
                    iterationsStarted,
                    iterationsSucceeded,
                    iterationsFailed,
                    iterationsCompleted);
        }
    }
}
