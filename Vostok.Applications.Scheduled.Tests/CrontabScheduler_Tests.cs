using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Time;

namespace Vostok.Applications.Scheduled.Tests
{
    [TestFixture]
    internal class CrontabScheduler_Tests
    {
        private DateTimeOffset from;

        [SetUp]
        public void TestSetup()
            => from = new DateTimeOffset(new DateTime(1900, 5, 4, 15, 32, 21, 100));

        [Test]
        public void Should_parse_schedules_with_five_components()
            => Scheduler.Crontab("* * * * *").ScheduleNext(from).Should().Be(from + 900.Milliseconds() + 38.Seconds());

        [Test]
        public void Should_parse_schedules_with_six_components()
            => Scheduler.Crontab("* * * * * *").ScheduleNext(from).Should().Be(from + 900.Milliseconds());
    }
}
