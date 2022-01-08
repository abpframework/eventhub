using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace EventHub.Events
{
    public class EventManager_Tests : EventHubDomainTestBase
    {
        private readonly EventHubTestData _testData;
        private readonly EventManager _eventManager;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<Event, Guid> _eventRepository;

        public EventManager_Tests()
        {
            _testData = GetRequiredService<EventHubTestData>();
            _eventManager = GetRequiredService<EventManager>();
            _organizationRepository = GetRequiredService<IRepository<Organization, Guid>>();
            _eventRepository = GetRequiredService<IRepository<Event, Guid>>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Event()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var volosoftOrganization = await _organizationRepository.GetAsync(
                    _testData.OrganizationVolosoftId
                );

                var @event = await _eventManager.CreateAsync(
                    volosoftOrganization,
                    "Introduction to the ABP Framework",
                    DateTime.Now.AddDays(1),
                    DateTime.Now.AddDays(1).AddHours(3),
                    "In this event, we will introduce the ABP Framework and explore the fundamental features."
                );

                @event.OrganizationId.ShouldBe(volosoftOrganization.Id);
                @event.Title.ShouldBe("Introduction to the ABP Framework");
            });
        }

        [Fact]
        public async Task Should_Update_The_Event_Capacity()
        {
            const int newCapacity = 42;

            await WithUnitOfWorkAsync(async () =>
            {
                var @event = await _eventRepository.GetAsync(
                    _testData.AbpMicroservicesFutureEventId);
                await _eventManager.SetCapacityAsync(
                    @event,
                    newCapacity
                );
            });
            
            var @event = await _eventRepository.GetAsync(
                _testData.AbpMicroservicesFutureEventId);
            @event.Capacity.ShouldBe(newCapacity);
        }
        
        [Fact]
        public async Task Should_Be_CountryId_And_City_Null_If_Event_Online()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features.");

            await _eventManager.SetLocationAsync(@event, true, "http://abp.io", Guid.NewGuid(), "Istanbul");

            @event.IsOnline.ShouldBeTrue();
            @event.OnlineLink.ShouldBe("http://abp.io");
            @event.CountryId.ShouldBeNull();
            @event.City.ShouldBeNull();
        }
    }
}
