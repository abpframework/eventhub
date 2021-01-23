using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace EventHub.Events
{
    public class EventAppService : EventHubAppService, IEventAppService
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly EventManager _eventManager;
        private readonly IRepository<Event, Guid> _eventRepository;

        public EventAppService(
            EventManager eventManager,
            IRepository<Organization, Guid> organizationRepository,
            IRepository<Event, Guid> eventRepository)
        {
            _eventManager = eventManager;
            _organizationRepository = organizationRepository;
            _eventRepository = eventRepository;
        }

        [Authorize]
        public async Task CreateAsync(CreateEventDto input)
        {
            var organization = await _organizationRepository.GetAsync(input.OrganizationId);

            if (organization.OwnerUserId != CurrentUser.GetId())
            {
                throw new BusinessException(EventHubErrorCodes.NotAuthorizedToCreateEventInThisOrganization)
                    .WithData("OrganizationName", organization.DisplayName);
            }

            var @event = await _eventManager.CreateAsync(
                organization,
                input.Title,
                input.StartTime,
                input.EndTime,
                input.Description
            );

            await _eventRepository.InsertAsync(@event);
        }
    }
}
