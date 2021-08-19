using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Countries;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Admin.Events
{
    [Authorize(EventHubPermissions.Events.Default)]
    public class EventAppService : EventHubAdminAppService, IEventAppService
    {
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IBlobContainer<EventCoverImageContainer> _eventBlobContainer;
        private readonly EventManager _eventManager;
        private readonly IRepository<Country, Guid> _countryRepository;


        public EventAppService(
            IRepository<Event, Guid> eventRepository,
            IRepository<EventRegistration, Guid> eventRegistrationRepository,
            IRepository<Organization, Guid> organizationRepository,
            IBlobContainer<EventCoverImageContainer> eventBlobContainer,
            EventManager eventManager, 
            IRepository<Country, Guid> countryRepository)
        {
            _eventRepository = eventRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _organizationRepository = organizationRepository;
            _eventBlobContainer = eventBlobContainer;
            _eventManager = eventManager;
            _countryRepository = countryRepository;
        }

        public async Task<EventDetailDto> GetAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);

            var eventDetailDto = ObjectMapper.Map<Event, EventDetailDto>(@event);
            eventDetailDto.CoverImageContent = await GetCoverImageAsync(id);

            return eventDetailDto;
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
                                             select @eventRegistration).Count()
                        })
                .WhereIf(!string.IsNullOrWhiteSpace(input.Title), x => x.Title.ToLower().Contains(input.Title.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.OrganizationDisplayName), x => x.OrganizationDisplayName.ToLower().Contains(input.OrganizationDisplayName.ToLower()))
                .WhereIf(input.MinStartTime.HasValue, x => x.StartTime >= input.MinStartTime)
                .WhereIf(input.MaxStartTime.HasValue, x => x.StartTime <= input.MaxStartTime)
                .WhereIf(input.MinAttendeeCount.HasValue, x => x.AttendeeCount >= input.MinAttendeeCount)
                .WhereIf(input.MaxAttendeeCount.HasValue, x => x.AttendeeCount <= input.MaxAttendeeCount);

            var totalCount = await AsyncExecuter.CountAsync(query);
            query = query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? EventConsts.DefaultSorting : input.Sorting);
            query = query.PageBy(input);

            var events = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<EventInListDto>(totalCount, events);
        }

        [Authorize(EventHubPermissions.Events.Update)]
        public async Task UpdateAsync(Guid id, UpdateEventDto input)
        {
            var @event = await _eventRepository.GetAsync(id);

            await _eventManager.SetLocationAsync(@event, input.IsOnline, input.OnlineLink, input.CountryId, input.City);
            @event.SetTitle(input.Title);
            @event.SetDescription(input.Description);
            @event.Language = input.Language;
            @event.SetTime(input.StartTime, @event.EndTime);
            await _eventManager.SetCapacityAsync(@event, input.Capacity);

            await SetCoverImageAsync(blobName: id.ToString(), input.CoverImageContent);

            await _eventRepository.UpdateAsync(@event);
        }

        public async Task<byte[]> GetCoverImageAsync(Guid id)
        {
            var blobName = id.ToString();

            return await _eventBlobContainer.GetAllBytesOrNullAsync(blobName);
        }

        public async Task<List<CountryLookupDto>> GetCountriesLookupAsync()
        {
            var countriesQueryable = await _countryRepository.GetQueryableAsync();

            var query = from country in countriesQueryable
                orderby country.Name
                select country;

            var countries = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Country>, List<CountryLookupDto>>(countries);
        }

        private async Task SetCoverImageAsync(string blobName, byte[] coverImageContent, bool overrideExisting = true)
        {
            await _eventBlobContainer.SaveAsync(blobName, coverImageContent, overrideExisting);
        }
    }
}
