using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace EventHub.Events
{
    public class EventManagerTests : EventHubDomainTestBase
    {
        private readonly EventHubTestData _testData;
        private readonly EventManager _eventManager;
        private readonly IRepository<Organization, Guid> _organizationRepository;

        public EventManagerTests()
        {
            _testData = GetRequiredService<EventHubTestData>();
            _eventManager = GetRequiredService<EventManager>();
            _organizationRepository = GetRequiredService<IRepository<Organization, Guid>>();
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
    }
}
