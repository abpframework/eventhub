using System;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace EventHub
{
    public class EventHubTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly ICurrentUser _currentUser;
        private readonly EventHubTestData _eventHubTestData;
        private readonly IClock _clock;

        public EventHubTestDataSeedContributor(
            EventHubTestData eventHubTestData,
            ICurrentUser currentUser,
            IRepository<Organization, Guid> organizationRepository,
            IRepository<Event, Guid> eventRepository,
            IClock clock)
        {
            _eventHubTestData = eventHubTestData;
            _currentUser = currentUser;
            _organizationRepository = organizationRepository;
            _eventRepository = eventRepository;
            _clock = clock;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await CreateOrganizationsAsync();
            await CreateEvents();
        }

        private async Task CreateOrganizationsAsync()
        {
            await _organizationRepository.InsertAsync(
                new Organization(
                    _eventHubTestData.OrganizationVolosoftId,
                    _currentUser.GetId(),
                    _eventHubTestData.OrganizationVolosoftName,
                    "Volosoft",
                    "Volosoft is producing software development tools for developers. We are organizing events related to the ABP.IO platform and general software development topic."
                )
            );

            await _organizationRepository.InsertAsync(
                new Organization(
                    _eventHubTestData.OrganizationDotnetEuropeId,
                    Guid.NewGuid(),
                    _eventHubTestData.OrganizationDotnetEuropeName,
                    "Dotnet Europe",
                    "Organizing events on Microsoft's .NET Platform in European Countries."
                )
            );
        }

        private async Task CreateEvents()
        {
            await _eventRepository.InsertAsync(
                new Event(
                    _eventHubTestData.AbpBlazorPastEventId,
                    _eventHubTestData.OrganizationVolosoftId,
                    _eventHubTestData.AbpBlazorPastEventUrlCode,
                    _eventHubTestData.AbpBlazorPastEventTitle,
                    _clock.Now.ClearTime().AddDays(-2).AddHours(15),
                    _clock.Now.ClearTime().AddDays(-2).AddHours(17),
                    "This is a past event about Blazor and the ABP Framework."
                )
            );

            await _eventRepository.InsertAsync(
                new Event(
                    _eventHubTestData.AbpMicroservicesFutureEventId,
                    _eventHubTestData.OrganizationVolosoftId,
                    _eventHubTestData.AbpMicroservicesFutureEventUrlCode,
                    _eventHubTestData.AbpMicroservicesFutureEventTitle,
                    _clock.Now.ClearTime().AddDays(1).AddHours(15),
                    _clock.Now.ClearTime().AddDays(1).AddHours(17),
                    "This is a future event about the ABP Framework and Microservices that is set for tomorrow."
                )
            );
        }
    }
}
