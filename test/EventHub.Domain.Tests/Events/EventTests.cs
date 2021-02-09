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

            exception.Code.ShouldBe(EventHubErrorCodes.EventEndTimeCantBeEarlierThanStartTime);
        }
        
        [Fact]
        public void Should_Be_CountryId_And_City_Null_If_Event_Online()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features.");

            @event.SetLocation(true, "http://abp.io", Guid.NewGuid(), "Istanbul");

            @event.IsOnline.ShouldBeTrue();
            @event.OnlineLink.ShouldBe("http://abp.io");
            @event.CountryId.ShouldBeNull();
            @event.City.ShouldBeNull();
        }
    }
}
