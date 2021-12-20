using System;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Timing;

namespace EventHub
{
    public class EventHubTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly EventHubTestData _eventHubTestData;
        private readonly IClock _clock;
        private readonly IIdentityUserRepository _userRepository;

        public EventHubTestDataSeedContributor(
            EventHubTestData eventHubTestData,
            IRepository<Organization, Guid> organizationRepository,
            IRepository<Event, Guid> eventRepository,
            IClock clock,
            IIdentityUserRepository userRepository)
        {
            _eventHubTestData = eventHubTestData;
            _organizationRepository = organizationRepository;
            _eventRepository = eventRepository;
            _clock = clock;
            _userRepository = userRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await CreateUsersAsync();
            await CreateOrganizationsAsync();
            await CreateEventsAsync();
        }

        private async Task CreateUsersAsync()
        {
            var adminUser = await _userRepository.FindByNormalizedUserNameAsync("ADMIN");
            _eventHubTestData.UserAdminId = adminUser.Id;

            await _userRepository.InsertAsync(
                new IdentityUser(
                    _eventHubTestData.UserJohnId,
                    _eventHubTestData.UserJohnUserName,
                    "john@abp.io"
                )
            );
        }

        private async Task CreateOrganizationsAsync()
        {
            await _organizationRepository.InsertAsync(
                new Organization(
                    _eventHubTestData.OrganizationVolosoftId,
                    _eventHubTestData.UserAdminId,
                    _eventHubTestData.OrganizationVolosoftName,
                    "Volosoft",
                    "Volosoft is producing software development tools for developers. We are organizing events related to the ABP.IO platform and general software development topic."
                )
            );

            await _organizationRepository.InsertAsync(
                new Organization(
                    _eventHubTestData.OrganizationDotnetEuropeId,
                    _eventHubTestData.UserJohnId,
                    _eventHubTestData.OrganizationDotnetEuropeName,
                    "Dotnet Europe",
                    "Organizing events on Microsoft's .NET Platform in European Countries."
                )
            );
        }

        private async Task CreateEventsAsync()
        {
            var pastEvent = new Event(
                _eventHubTestData.AbpBlazorPastEventId,
                _eventHubTestData.OrganizationVolosoftId,
                _eventHubTestData.AbpBlazorPastEventUrlCode,
                _eventHubTestData.AbpBlazorPastEventTitle,
                _clock.Now.ClearTime().AddDays(-2).AddHours(15),
                _clock.Now.ClearTime().AddDays(-2).AddHours(17),
                "This is a past event about Blazor and the ABP Framework."
            );
            pastEvent.Publish();
            await _eventRepository.InsertAsync(
                pastEvent
            );

            var futureEvent = new Event(
                _eventHubTestData.AbpMicroservicesFutureEventId,
                _eventHubTestData.OrganizationVolosoftId,
                _eventHubTestData.AbpMicroservicesFutureEventUrlCode,
                _eventHubTestData.AbpMicroservicesFutureEventTitle,
                _clock.Now.ClearTime().AddDays(1).AddHours(15),
                _clock.Now.ClearTime().AddDays(1).AddHours(17),
                "This is a future event about the ABP Framework and Microservices that is set for tomorrow."
            );
            futureEvent.Capacity = 1;
            futureEvent.Publish();
            await _eventRepository.InsertAsync(
               futureEvent
            );
        }
    }
}
