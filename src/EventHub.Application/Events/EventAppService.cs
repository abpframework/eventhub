using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Countries;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace EventHub.Events
{
    public class EventAppService : EventHubAppService, IEventAppService
    {
        private readonly EventManager _eventManager;
        private readonly EventRegistrationManager _eventRegistrationManager;
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Country, Guid> _countriesRepository;
        private readonly IBlobContainer<EventCoverImageContainer> _eventBlobContainer;

        public EventAppService(
            EventManager eventManager,
            EventRegistrationManager eventRegistrationManager,
            IEventRegistrationRepository eventRegistrationRepository,
            IEventRepository eventRepository,
            IRepository<Organization, Guid> organizationRepository,
            IUserRepository userRepository,
            IRepository<Country, Guid> countriesRepository,
            IBlobContainer<EventCoverImageContainer> eventBlobContainer)
        {
            _eventManager = eventManager;
            _eventRegistrationManager = eventRegistrationManager;
            _eventRegistrationRepository = eventRegistrationRepository;
            _eventRepository = eventRepository;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _countriesRepository = countriesRepository;
            _eventBlobContainer = eventBlobContainer;
        }

        [Authorize]
        public async Task<EventDto> CreateAsync(CreateEventDto input)
        {
            var organization = await _organizationRepository.GetAsync(input.OrganizationId);

            if (organization.OwnerUserId != CurrentUser.GetId())
            {
                throw new AbpAuthorizationException(
                    L["EventHub:NotAuthorizedToCreateEventInThisOrganization", organization.DisplayName],
                    EventHubErrorCodes.NotAuthorizedToCreateEventInThisOrganization
                );
            }

            var @event = await _eventManager.CreateAsync(
                organization,
                input.Title,
                input.StartTime,
                input.EndTime,
                input.Description
            );

            await _eventManager.SetLocationAsync(@event, input.IsOnline, input.OnlineLink, input.CountryId, input.City);
            @event.Language = input.Language;

            await _eventManager.SetCapacityAsync(@event, input.Capacity);

            if (input.CoverImageStreamContent != null && input.CoverImageStreamContent.ContentLength > 0)
            {
                await SaveCoverImageAsync(@event.Id, input.CoverImageStreamContent);
            }

            await _eventRepository.InsertAsync(@event);

            return ObjectMapper.Map<Event, EventDto>(@event);
        }

        public async Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input)
        {
            var eventQueryable = await _eventRepository.GetQueryableAsync();
            var eventRegistrationQueryable = await _eventRegistrationRepository.GetQueryableAsync();
            var organizationQueryable = await _organizationRepository.GetQueryableAsync();

            var query = from @event in eventQueryable
                join organization in organizationQueryable on @event.OrganizationId equals organization.Id
                select new {@event, organization};

            if (input.RegisteredUserId.HasValue)
            {
                var registeredEvent = eventRegistrationQueryable
                    .Where(x => x.UserId == input.RegisteredUserId)
                    .Select(s => s.EventId);

                var eventIds = await AsyncExecuter.ToListAsync(registeredEvent);
                query = query.Where(x => eventIds.Contains(x.@event.Id));
            }

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

            if (input.IsOnline.HasValue)
            {
                query = query.Where(i => i.@event.IsOnline == input.IsOnline);
            }

            if (!input.Language.IsNullOrWhiteSpace())
            {
                query = query.Where(i => i.@event.Language == input.Language);
            }

            if (input.CountryId.HasValue && input.CountryId != Guid.Empty)
            {
                query = query.Where(i => i.@event.CountryId == input.CountryId);
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

            var events = items.Select(
                i =>
                {
                    var dto = ObjectMapper.Map<Event, EventInListDto>(i.@event);
                    dto.OrganizationName = i.organization.Name;
                    dto.OrganizationDisplayName = i.organization.DisplayName;
                    dto.IsLiveNow = now.IsBetween(i.@event.StartTime, i.@event.EndTime);
                    dto.Country = i.@event.CountryName;
                    return dto;
                }
            ).ToList();

            return new PagedResultDto<EventInListDto>(totalCount, events);
        }

        public async Task<EventDetailDto> GetByUrlCodeAsync(string urlCode)
        {
            var @event = await _eventRepository.GetAsync(x => x.UrlCode == urlCode);
            var organization = await _organizationRepository.GetAsync(@event.OrganizationId);

            var dto = ObjectMapper.Map<Event, EventDetailDto>(@event);

            dto.OrganizationId = organization.Id;
            dto.OrganizationName = organization.Name;
            dto.OrganizationDisplayName = organization.DisplayName;
            
            var user = await _userRepository.GetAsync(organization.OwnerUserId);
            dto.OwnerUserName = user.UserName;
            dto.OwnerEmail = user.Email;

            return dto;
        }

        [Authorize]
        public async Task<EventLocationDto> GetLocationAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            var user = await _userRepository.GetAsync(CurrentUser.GetId());

            var dto = ObjectMapper.Map<Event, EventLocationDto>(@event);

            dto.IsRegistered = await _eventRegistrationManager.IsRegisteredAsync(@event, user);

            if (!dto.IsRegistered)
            {
                dto.OnlineLink = null;
                dto.City = null;
            }

            if (dto.IsRegistered && @event.CountryId.HasValue)
            {
                dto.Country = (await _countriesRepository.GetAsync(@event.CountryId.Value)).Name;
            }

            return dto;
        }

        public async Task<List<CountryLookupDto>> GetCountriesLookupAsync()
        {
            var countriesQueryable = await _countriesRepository.GetQueryableAsync();

            var query = from country in countriesQueryable
                orderby country.Name
                select country;

            var countries = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Country>, List<CountryLookupDto>>(countries);
        }

        public async Task<bool> IsEventOwnerAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            var organization = await _organizationRepository.GetAsync(@event.OrganizationId);

            return CurrentUser.Id.HasValue && organization.OwnerUserId == CurrentUser.GetId();
        }

        [Authorize]
        public async Task UpdateAsync(Guid id, UpdateEventDto input)
        {
            var @event = await _eventRepository.GetAsync(id);
            var organization = await _organizationRepository.GetAsync(@event.OrganizationId);

            if (organization.OwnerUserId != CurrentUser.GetId())
            {
                throw new AbpAuthorizationException(
                    L["EventHub:NotAuthorizedToUpdateEvent", @event.Title],
                    EventHubErrorCodes.NotAuthorizedToUpdateEvent
                );
            }

            await _eventManager.SetLocationAsync(@event, input.IsOnline, input.OnlineLink, input.CountryId, input.City);
            @event.SetTitle(input.Title);
            @event.SetDescription(input.Description);
            @event.Language = input.Language;
            @event.SetTime(input.StartTime, input.EndTime);
            await _eventManager.SetCapacityAsync(@event, input.Capacity);
            
            if (input.CoverImageStreamContent != null && input.CoverImageStreamContent.ContentLength > 0)
            {
                await SaveCoverImageAsync(@event.Id, input.CoverImageStreamContent);
            }
            
            await _eventRepository.UpdateAsync(@event);
        }
        
        public async Task<IRemoteStreamContent> GetCoverImageAsync(Guid id)
        {
            var blobName = id.ToString();

            var coverImageStream = await _eventBlobContainer.GetOrNullAsync(blobName);
            
            if (coverImageStream is null)
            {
                return null;
            }

            return new RemoteStreamContent(coverImageStream, blobName);
        }

        private async Task SaveCoverImageAsync(Guid id, IRemoteStreamContent streamContent)
        {
            var blobName = id.ToString();

            await _eventBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting: true);
        }
    }
}