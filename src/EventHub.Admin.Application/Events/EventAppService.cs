using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Countries;
using EventHub.Events;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Admin.Events
{
    [Authorize(EventHubPermissions.Events.Default)]
    public class EventAppService : EventHubAdminAppService, IEventAppService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IBlobContainer<EventCoverImageContainer> _eventBlobContainer;
        private readonly EventManager _eventManager;
        private readonly IRepository<Country, Guid> _countryRepository;

        public EventAppService(
            IEventRepository eventRepository,
            IBlobContainer<EventCoverImageContainer> eventBlobContainer,
            EventManager eventManager,
            IRepository<Country, Guid> countryRepository)
        {
            _eventRepository = eventRepository;
            _eventBlobContainer = eventBlobContainer;
            _eventManager = eventManager;
            _countryRepository = countryRepository;
        }

        public async Task<EventDetailDto> GetAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            var eventDetailDto = ObjectMapper.Map<Event, EventDetailDto>(@event);

            return eventDetailDto;
        }

        public async Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input)
        {
            var totalCount = await _eventRepository.GetCountAsync(input.Title, input.OrganizationDisplayName, input.MinAttendeeCount,
                input.MaxAttendeeCount, input.MinStartTime, input.MaxStartTime);

            var items = await _eventRepository.GetListAsync(input.Sorting, input.SkipCount, input.MaxResultCount,
                input.Title, input.OrganizationDisplayName, input.MinAttendeeCount, input.MaxAttendeeCount,
                input.MinStartTime, input.MaxStartTime);

            var events = ObjectMapper.Map<List<EventWithDetails>, List<EventInListDto>>(items);

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
            @event.SetTime(input.StartTime, input.EndTime);
            await _eventManager.SetCapacityAsync(@event, input.Capacity);

            if (input.CoverImageStreamContent != null && input.CoverImageStreamContent.ContentLength > 0)
            {
                await SetCoverImageAsync(blobName: id.ToString(), input.CoverImageStreamContent);
            }
            else
            {
                await DeleteCoverImageAsync(blobName: id.ToString());
            }

            await _eventRepository.UpdateAsync(@event);
        }

        [AllowAnonymous]
        public async Task<IRemoteStreamContent> GetCoverImageAsync(Guid id)
        {
            var blobName = id.ToString();
            var coverImageStream = await _eventBlobContainer.GetOrNullAsync(blobName);

            if (coverImageStream == null)
            {
                return null;
            }

            return new RemoteStreamContent(coverImageStream);
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

        private async Task SetCoverImageAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
            await _eventBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        }
        
        private async Task DeleteCoverImageAsync(string blobName)
        {
            await _eventBlobContainer.DeleteAsync(blobName);
        }
    }
}
