using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace EventHub.Events
{
    public class EventAppService : EventHubAppService, IEventAppService
    {
        private readonly EventManager _eventManager;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly NewEventNotifier _newEventNotifier;

        public EventAppService(
            EventManager eventManager,
            IRepository<Organization, Guid> organizationRepository,
            IRepository<Event, Guid> eventRepository, 
            NewEventNotifier newEventNotifier)
        {
            _eventManager = eventManager;
            _organizationRepository = organizationRepository;
            _eventRepository = eventRepository;
            _newEventNotifier = newEventNotifier;
        }

        [Authorize]
        public async Task<EventDto> CreateAsync(CreateEventDto input)
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

            @event.IsOnline = input.IsOnline;
            @event.Capacity = input.Capacity;
            @event.IsEmailSentToMembers = true;
            
            await _eventRepository.InsertAsync(@event);

            try
            {
                await _newEventNotifier.NotifyAsync(organization, @event);
            }
            catch (Exception e)
            {
                @event.IsEmailSentToMembers = false;
                Logger.LogError($"An error occurred while sending an email to the members of the {organization.Name} organization after the new event was created. Error message: {e.Message}");
            }

            return ObjectMapper.Map<Event, EventDto>(@event);
        }

        public async Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input)
        {
            var eventQueryable = await _eventRepository.GetQueryableAsync();
            var organizationQueryable = await _organizationRepository.GetQueryableAsync();

            var query = from @event in eventQueryable
                join organization in organizationQueryable on @event.OrganizationId equals organization.Id
                select new {@event, organization};

            if (input.MinDate.HasValue)
            {
                query = query.Where(i => i.@event.EndTime >= input.MinDate.Value);
            }

            if (input.MaxDate.HasValue)
            {
                query = query.Where(i => i.@event.EndTime <= input.MaxDate.Value);
            }

            if (input.OrganizationId.HasValue)
            {
                query = query.Where(i => i.@event.OrganizationId == input.OrganizationId.Value);
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.PageBy(input);

            if (input.MaxDate.HasValue && !input.MinDate.HasValue)
            {
                query = query.OrderByDescending(x => x.@event.StartTime);
            }
            else
            {
                query = query.OrderBy(x => x.@event.StartTime);
            }

            var items = await AsyncExecuter.ToListAsync(query);
            var now = Clock.Now;

            var dtos = items.Select(
                i =>
                {
                    var dto = ObjectMapper.Map<Event, EventInListDto>(i.@event);
                    dto.OrganizationName = i.organization.Name;
                    dto.OrganizationDisplayName = i.organization.DisplayName;
                    dto.IsLiveNow = now.IsBetween(i.@event.StartTime, i.@event.EndTime);
                    return dto;
                }
            ).ToList();

            return new PagedResultDto<EventInListDto>(totalCount, dtos);
        }

        public async Task<EventDetailDto> GetByUrlCodeAsync(string urlCode)
        {
            var @event = await _eventRepository.GetAsync(x => x.UrlCode == urlCode);
            var organization = await _organizationRepository.GetAsync(@event.OrganizationId);

            var dto = ObjectMapper.Map<Event, EventDetailDto>(@event);

            dto.OrganizationName = organization.Name;
            dto.OrganizationDisplayName = organization.DisplayName;

            return dto;
        }
    }
}
