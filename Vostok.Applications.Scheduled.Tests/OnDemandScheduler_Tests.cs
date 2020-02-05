using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Applications.Scheduled.Tests
{
    [TestFixture]
    internal class OnDemandScheduler_Tests
    {
        private Action signal;
        private IScheduler scheduler;

        [SetUp]
        public void TestSetup()
            => scheduler = Scheduler.OnDemand(out signal);

        [Test]
        public void Should_not_schedule_until_demanded()
            => scheduler.ScheduleNext(DateTimeOffset.Now).Should().BeNull();

        [Test]
        public void Should_schedule_once_when_demanded()
        {
            signal();

            var from = DateTimeOffset.Now;

            scheduler.ScheduleNext(from).Should().Be(from);
            scheduler.ScheduleNext(from).Should().BeNull();
            scheduler.ScheduleNext(from).Should().BeNull();
        }
    }
}
