using System;
using System.Threading.Tasks;
using EventHub.Organizations.Plans;
using NSubstitute;
using Shouldly;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Timing;
using Xunit;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationManager_UnitTests
    {
        [Fact]
        public void IsPastEvent()
        {
            var clock = Substitute.For<IClock>();
            clock.Now.Returns(DateTime.Now);

            var registrationManager = new EventRegistrationManager(
                null, null, clock, null
            );

            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now.AddDays(-10), // Start time
                DateTime.Now.AddDays(-9), // End time
                "In this event, we will introduce the ABP Framework..."
            );

            registrationManager.IsPastEvent(@event).ShouldBeTrue();
        }

        [Fact]
        public async Task RegisterAsync()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework..."
            );

            var user = new IdentityUser(Guid.NewGuid(), "john", "john@abp.io");

            var repository = Substitute.For<IEventRegistrationRepository>();
            repository.ExistsAsync(@event.Id, user.Id).Returns(Task.FromResult(false));

            var guidGenerator = SimpleGuidGenerator.Instance;

            var clock = Substitute.For<IClock>();
            clock.Now.Returns(DateTime.Now);

            var planFeatureManager = Substitute.For<PlanFeatureManager>(null, null, null, null, null);
            planFeatureManager.CanRegisterToEventAsync(@event).Returns(Task.FromResult(true));

            var registrationManager = new EventRegistrationManager(
                repository, guidGenerator, clock, planFeatureManager
            );

            await registrationManager.RegisterAsync(@event, user);

            await repository
                .Received()
                .InsertAsync(
                    Arg.Is<EventRegistration>(er => er.EventId == @event.Id && er.UserId == user.Id)
            );
        }
    }
}
