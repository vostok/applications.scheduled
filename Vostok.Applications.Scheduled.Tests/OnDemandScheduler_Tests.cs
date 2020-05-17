using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Commons.Testing;

namespace Vostok.Applications.Scheduled.Tests
{
    [TestFixture]
    internal class OnDemandScheduler_Tests
    {
        private Action signal;
        private Func<Task> signalWithFeedback;
        private IScheduler scheduler;
        private IScheduler schedulerWithFeedback;

        [SetUp]
        public void TestSetup()
        {
            scheduler = Scheduler.OnDemand(out signal);
            schedulerWithFeedback = Scheduler.OnDemandWithFeedback(out signalWithFeedback);
        }

        [Test]
        public void Should_not_schedule_until_demanded()
            => scheduler.ScheduleNext(DateTimeOffset.Now).Should().BeNull();

        [Test]
        public void Should_schedule_once_when_demanded_without_feedback()
        {
            signal();

            var from = DateTimeOffset.Now;

            scheduler.ScheduleNext(from).Should().Be(from);
            scheduler.ScheduleNext(from).Should().BeNull();
            scheduler.ScheduleNext(from).Should().BeNull();
        }

        [Test]
        public void Should_schedule_once_when_demanded_with_feedback()
        {
            signalWithFeedback();

            var from = DateTimeOffset.Now;

            schedulerWithFeedback.ScheduleNext(from).Should().Be(from);
            schedulerWithFeedback.ScheduleNext(from).Should().BeNull();
            schedulerWithFeedback.ScheduleNext(from).Should().BeNull();
        }

        [Test]
        public void Should_produce_feedback_when_iteration_ends_successfully()
        {
            var tasks = new List<Task>
            {
                signalWithFeedback(), 
                signalWithFeedback(), 
                signalWithFeedback()
            };

            schedulerWithFeedback.OnSuccessfulIteration(Substitute.For<IScheduler>());

            Action assertion = () =>
            {
                foreach (var task in tasks)
                {
                    task.IsCompleted.Should().BeTrue();
                    task.GetAwaiter().GetResult();
                }
            };

            assertion.ShouldPassIn(5.Seconds());
        }

        [Test]
        public void Should_produce_feedback_when_iteration_fails()
        {
            var tasks = new List<Task>
            {
                signalWithFeedback(),
                signalWithFeedback(),
                signalWithFeedback()
            };

            var error = new Exception();

            schedulerWithFeedback.OnFailedIteration(Substitute.For<IScheduler>(), error);

            Action assertion = () =>
            {
                foreach (var task in tasks)
                {
                    task.IsFaulted.Should().BeTrue();
                    task.Exception.InnerExceptions.Should().ContainSingle().Which.Should().BeSameAs(error);
                }
            };

            assertion.ShouldPassIn(5.Seconds());
        }
    }
}
