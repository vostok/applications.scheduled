using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Time;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace Vostok.Applications.Scheduled.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class ScheduledActionRunner_Tests
{
    private const string ActionName = "TestAction";

    [Test]
    public void RunAsync_should_throw_payload_exception_when_CrashOnPayloadException_true()
    {
        var options = new ScheduledActionOptions
        {
            CrashOnPayloadException = true
        };

        var action = new ScheduledAction(
            ActionName,
            Scheduler.Periodical(50.Milliseconds()),
            options,
            _ => throw new InvalidOperationException("Error"));

        var runner = new ScheduledActionRunner(action, new SilentLog(), new DevNullTracer(), diagnostics: null);

        var run = async () => await runner.RunAsync(GetCancellationToken());

        run.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void RunAsync_should_not_throw_payload_exception_when_CrashOnPayloadException_false()
    {
        var options = new ScheduledActionOptions
        {
            CrashOnPayloadException = false
        };

        var action = new ScheduledAction(
            ActionName,
            Scheduler.Periodical(50.Milliseconds()),
            options,
            _ => throw new InvalidOperationException("Error"));

        var runner = new ScheduledActionRunner(action, new SilentLog(), new DevNullTracer(), diagnostics: null);

        var run = async () => await runner.RunAsync(GetCancellationToken());

        run.Should().NotThrow();
    }

    [Test]
    public void RunAsync_should_throw_payload_OutOfMemoryException_even_when_CrashOnPayloadException_false()
    {
        var options = new ScheduledActionOptions
        {
            CrashOnPayloadException = false
        };

        var action = new ScheduledAction(
            ActionName,
            Scheduler.Periodical(50.Milliseconds()),
            options,
            _ => throw new OutOfMemoryException());

        var runner = new ScheduledActionRunner(action, new SilentLog(), new DevNullTracer(), diagnostics: null);

        var run = async () => await runner.RunAsync(GetCancellationToken());

        run.Should().Throw<OutOfMemoryException>();
    }

    [Test]
    public void RunAsync_should_not_throw_payload_OutOfMemoryException_when_CrashOnPayloadOutOfMemoryException_false()
    {
        var options = new ScheduledActionOptions
        {
            CrashOnPayloadException = false,
            CrashOnPayloadOutOfMemoryException = false
        };

        var action = new ScheduledAction(
            ActionName,
            Scheduler.Periodical(50.Milliseconds()),
            options,
            _ => throw new OutOfMemoryException());

        var runner = new ScheduledActionRunner(action, new SilentLog(), new DevNullTracer(), diagnostics: null);

        var run = async () => await runner.RunAsync(GetCancellationToken());

        run.Should().NotThrow();
    }

    [Test]
    public void RunAsync_should_throw_scheduler_exception_when_CrashOnSchedulerException_true()
    {
        var options = new ScheduledActionOptions
        {
            CrashOnSchedulerException = true
        };

        var action = new ScheduledAction(
            ActionName,
            new ExceptionThrowingScheduler(new InvalidOperationException("Error")),
            options,
            _ => Task.CompletedTask);

        var runner = new ScheduledActionRunner(action, new SilentLog(), new DevNullTracer(), diagnostics: null);

        var run = async () => await runner.RunAsync(GetCancellationToken());

        run.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void RunAsync_should_not_throw_scheduler_exception_when_CrashOnSchedulerException_false()
    {
        var options = new ScheduledActionOptions
        {
            CrashOnSchedulerException = false
        };

        var action = new ScheduledAction(
            ActionName,
            new ExceptionThrowingScheduler(new InvalidOperationException("Error")),
            options,
            _ => Task.CompletedTask);

        var runner = new ScheduledActionRunner(action, new SilentLog(), new DevNullTracer(), diagnostics: null);

        var run = async () => await runner.RunAsync(GetCancellationToken());

        run.Should().NotThrow();
    }

    [Test]
    public void RunAsync_should_throw_scheduler_OutOfMemoryException_even_when_CrashOnSchedulerException_false()
    {
        var options = new ScheduledActionOptions
        {
            CrashOnSchedulerException = false
        };

        var action = new ScheduledAction(
            ActionName,
            new ExceptionThrowingScheduler(new OutOfMemoryException()),
            options,
            _ => Task.CompletedTask);

        var runner = new ScheduledActionRunner(action, new SilentLog(), new DevNullTracer(), diagnostics: null);

        var run = async () => await runner.RunAsync(GetCancellationToken());

        run.Should().Throw<OutOfMemoryException>();
    }

    [Test]
    public void RunAsync_should_not_throw_scheduler_OutOfMemoryException_when_CrashOnSchedulerOutOfMemoryException_false()
    {
        var options = new ScheduledActionOptions
        {
            CrashOnSchedulerException = false,
            CrashOnSchedulerOutOfMemoryException = false
        };

        var action = new ScheduledAction(
            ActionName,
            new ExceptionThrowingScheduler(new OutOfMemoryException()),
            options,
            _ => Task.CompletedTask);

        var runner = new ScheduledActionRunner(action, new SilentLog(), new DevNullTracer(), diagnostics: null);

        var run = async () => await runner.RunAsync(GetCancellationToken());

        run.Should().NotThrow();
    }

    private static CancellationToken GetCancellationToken()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1.Seconds());
        return cts.Token;
    }

    private class ExceptionThrowingScheduler : IScheduler
    {
        private readonly Exception exception;

        public ExceptionThrowingScheduler(Exception exception)
        {
            this.exception = exception;
        }

        public DateTimeOffset? ScheduleNext(DateTimeOffset from) => throw exception;
    }
}