using System;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace EventHub.Events
{
    public class EventTests
    {
        [Fact]
        public void Should_Not_Allow_End_Time_To_Be_Earlier_Than_Start_Time()
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
                    "In this event, we will introduce the ABP Framework and explore the fundamental features."
                );
            });

            exception.Code.ShouldBe(EventHubErrorCodes.EndTimeCantBeEarlierThanStartTime);
        }
    }
}
