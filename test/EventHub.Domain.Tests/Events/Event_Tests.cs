using System;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace EventHub.Events
{
    public class Event_Tests
    {
        [Fact]
        public void Should_Create_A_Valid_Event()
        {
            new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddHours(2),
                "In this event, we will introduce the ABP Framework..."
            );
        }

        [Fact]
        public void Should_Update_Event_Time()
        {
            // ARRANGE
            var evnt = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddHours(2),
                "In this event, we will introduce the ABP Framework..."
            );

            var newStartTime = DateTime.Now.AddHours(1);
            var newEndTime = DateTime.Now.AddHours(2);

            //ACT
            evnt.SetTime(newStartTime, newEndTime);

            //ASSERT
            evnt.StartTime.ShouldBe(newStartTime);
            evnt.EndTime.ShouldBe(newEndTime);
        }

        [Fact]
        public void Should_Not_Allow_End_Time_Earlier_Than_Start_Time()
        {
            var exception = Assert.Throws<BusinessException>(() =>
            {
                new Event(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "1a8j3v0d",
                    "Introduction to the ABP Framework",
                    DateTime.Now,
                    DateTime.Now.AddDays(-2),
                    "In this event, we will introduce the ABP Framework..."
                );
            });

            exception.Code.ShouldBe(
                EventHubErrorCodes.EndTimeCantBeEarlierThanStartTime);
        }
    }
}
