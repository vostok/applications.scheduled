using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Vostok.Applications.Scheduled.Diagnostics
{
    internal class ScheduledActionStatistics
    {
        internal ScheduledActionStatistics(
            bool currentlyExecuting, 
            TimeSpan currentExecutionDuration,
            TimeSpan? timeToNextExecution,
            DateTimeOffset? nextExecution, 
            DateTimeOffset? lastStart, 
            DateTimeOffset? lastFinish, 
            DateTimeOffset? lastError, 
            string lastErrorMessage,
            bool lastIterationSuccessful,
            TimeSpan lastDuration, 
            TimeSpan totalDuration, 
            TimeSpan averageDuration, 
            int iterationsStarted, 
            int iterationsSucceeded, 
            int iterationsFailed, 
            int iterationsCompleted)
        {
            CurrentlyExecuting = currentlyExecuting;
            CurrentExecutionDuration = currentExecutionDuration;
            TimeToNextExecution = timeToNextExecution;
            NextExecution = nextExecution;
            LastStart = lastStart;
            LastFinish = lastFinish;
            LastError = lastError;
            LastErrorMessage = lastErrorMessage;
            LastIterationSuccessful = lastIterationSuccessful;
            LastDuration = lastDuration;
            TotalDuration = totalDuration;
            AverageDuration = averageDuration;
            IterationsStarted = iterationsStarted;
            IterationsSucceeded = iterationsSucceeded;
            IterationsFailed = iterationsFailed;
            IterationsCompleted = iterationsCompleted;
        }

        public bool CurrentlyExecuting { get; }

        public TimeSpan CurrentExecutionDuration { get; }

        public TimeSpan? TimeToNextExecution { get; }

        public DateTimeOffset? NextExecution { get; }

        public DateTimeOffset? LastStart { get; }
        
        public DateTimeOffset? LastFinish { get; }

        public DateTimeOffset? LastError { get; }

        public string LastErrorMessage { get; }

        public bool LastIterationSuccessful { get; }

        public TimeSpan LastDuration { get; }

        public TimeSpan TotalDuration { get; }

        public TimeSpan AverageDuration { get; }

        public int IterationsStarted { get; }
        
        public int IterationsSucceeded { get; }

        public int IterationsFailed { get; }

        public int IterationsCompleted { get; }
    }
}
