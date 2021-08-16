using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Admin.Events
{
    //[Authorize(EventHubPermissions.Events.Default)]
    public class EventAppService : EventHubAdminAppService, IEventAppService
    {
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;

        public EventAppService(
            IRepository<Event, Guid> eventRepository,
            IRepository<EventRegistration, Guid> eventRegistrationRepository,
            IRepository<Organization, Guid> organizationRepository
        )
        {
            _eventRepository = eventRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _organizationRepository = organizationRepository;
        }

        public async Task<EventDetailDto> GetAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);

            return ObjectMapper.Map<Event, EventDetailDto>(@event);
        }

        public async Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input)
        {
            var eventQueryable = await _eventRepository.GetQueryableAsync();
            var eventRegistrationQueryable = await _eventRegistrationRepository.GetQueryableAsync();
            var organizationQueryable = await _organizationRepository.GetQueryableAsync();

            var query = (from @event in eventQueryable
                        join organization in organizationQueryable on @event.OrganizationId equals organization.Id
                        select new EventInListDto
                        {
                            Id = @event.Id,
                            Title = @event.Title,
                            StartTime = @event.StartTime,
                            OrganizationDisplayName = organization.DisplayName,
                            AttendeeCount = (from eventRegistration in eventRegistrationQueryable
                                             where eventRegistration.EventId == @event.Id
                                             group @event by @event.Id into g
                                             select g.Key).Count()
                        })
                .WhereIf(!string.IsNullOrWhiteSpace(input.Title), x => x.Title.ToLower().Contains(input.Title.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.OrganizationDisplayName), x => x.OrganizationDisplayName.ToLower().Contains(input.OrganizationDisplayName.ToLower()))
                .WhereIf(input.StartTime.HasValue, x => x.StartTime > input.StartTime)
                .WhereIf(input.MinAttendeeCount.HasValue, x => x.AttendeeCount >= input.MinAttendeeCount)
                .WhereIf(input.MaxAttendeeCount.HasValue, x => x.AttendeeCount <= input.MaxAttendeeCount);

            var totalCount = await AsyncExecuter.CountAsync(query);
            query = query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? EventConsts.DefaultSorting : input.Sorting);
            query = query.PageBy(input);

            var events = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<EventInListDto>(totalCount, events);
        }

        //[Authorize(EventHubPermissions.Events.Update)]
        public async Task UpdateAsync(Guid id, UpdateEventDto input)
        {
            var @event = await _eventRepository.GetAsync(id);

            @event.SetTitle(input.Title);
            @event.SetTime(input.StartTime, @event.EndTime);

            await _eventRepository.UpdateAsync(@event);
        }
    }
}
