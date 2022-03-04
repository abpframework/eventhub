using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using EventHub.Countries;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
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
                Clock.Normalize(input.StartTime),
                Clock.Normalize(input.EndTime),
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
                where !@event.IsDraft
                select new { @event, organization };

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

            var items = (await AsyncExecuter.ToListAsync(query)).Select(a => (a.@event, a.organization)).ToList();

            var events = GetEventInListDtoFromEventAndOrganizationTupleList(items);

            return new PagedResultDto<EventInListDto>(totalCount, events);
        }

        [Authorize]
        public async Task<List<EventInListDto>> GetDraftEventsByCurrentUser()
        {
            var eventQueryable = await _eventRepository.GetQueryableAsync();
            var organizationQueryable = await _organizationRepository.GetQueryableAsync();

            var userId = CurrentUser.GetId();
            var query = from @event in eventQueryable
                join organization in organizationQueryable on @event.OrganizationId equals organization.Id
                where organization.OwnerUserId == userId && @event.IsDraft
                select new { @event, organization };

            var items = (await AsyncExecuter.ToListAsync(query)).Select(a => (a.@event, a.organization)).ToList();

            return GetEventInListDtoFromEventAndOrganizationTupleList(items);
        }

        public async Task<EventDetailDto> GetByUrlCodeAsync(string urlCode)
        {
            var @event = await _eventRepository.GetAsync(x => x.UrlCode == urlCode, true);
            if (@event.IsDraft)
            {
                if (!CurrentUser.Id.HasValue)
                {
                    throw new AuthenticationException();
                }

                await CheckIfValidOwnerAsync(@event);
            }
            
            var organization = await _organizationRepository.GetAsync(@event.OrganizationId);

            var dto = ObjectMapper.Map<Event, EventDetailDto>(@event);

            dto.OrganizationId = organization.Id;
            dto.OrganizationName = organization.Name;
            dto.OrganizationDisplayName = organization.DisplayName;

            var user = await _userRepository.GetAsync(organization.OwnerUserId);
            if (@event.IsDraft)
            {
                if (user.Id != CurrentUser.GetId())
                {
                    throw new AbpAuthorizationException();
                }
            }
            
            dto.OwnerUserName = user.UserName;
            dto.OwnerEmail = user.Email;

            foreach (var speaker in from track in dto.Tracks from session in track.Sessions from speaker in session.Speakers select speaker)
            {
                speaker.UserName = (await _userRepository.GetAsync(speaker.UserId)).UserName;
            }

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
            await CheckIfValidOwnerAsync(@event);

            await _eventManager.SetLocationAsync(@event, input.IsOnline, input.OnlineLink, input.CountryId, input.City);
            @event.SetTitle(input.Title);
            @event.SetDescription(input.Description);
            @event.Language = input.Language;
            @event.SetTime(Clock.Normalize(input.StartTime), Clock.Normalize(input.EndTime));
            await _eventManager.SetCapacityAsync(@event, input.Capacity);

            if (input.CoverImageStreamContent != null && input.CoverImageStreamContent.ContentLength > 0)
            {
                await SaveCoverImageAsync(@event.Id, input.CoverImageStreamContent);
            }

            await _eventRepository.UpdateAsync(@event);
        }

        [Authorize]
        public async Task<string> PublishAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            await CheckIfValidOwnerAsync(@event);

            @event.Publish();
            
            await _eventRepository.UpdateAsync(@event, true);

            return @event.UrlCode;
        }

        [Authorize]
        public async Task AddTrackAsync(Guid id, AddTrackDto input)
        {
            var @event = await _eventRepository.GetAsync(id, true);
            await CheckIfValidOwnerAsync(@event);

            await _eventManager.AddTrackAsync(@event, input.Name);

            await _eventRepository.UpdateAsync(@event);
        }

        [Authorize]
        public async Task UpdateTrackAsync(Guid id, Guid trackId, UpdateTrackDto input)
        {
            var @event = await _eventRepository.GetAsync(id, true);
            await CheckIfValidOwnerAsync(@event);

            @event.UpdateTrack(
                trackId,
                input.Name
            );

            await _eventRepository.UpdateAsync(@event);
        }

        [Authorize]
        public async Task DeleteTrackAsync(Guid id, Guid trackId)
        {
            var @event = await _eventRepository.GetAsync(id, true);
            await CheckIfValidOwnerAsync(@event);

            @event.RemoveTrack(trackId);
            
            await _eventRepository.UpdateAsync(@event);
        }

        [Authorize]
        public async Task AddSessionAsync(Guid id, Guid trackId, AddSessionDto input)
        {
            var @event = await _eventRepository.GetAsync(id);
            await CheckIfValidOwnerAsync(@event);
            
            @event.AddSession(
                trackId,
                GuidGenerator.Create(),
                input.Title,
                input.Description,
                Clock.Normalize(input.StartTime),
                Clock.Normalize(input.EndTime),
                input.Language,
                await GetUserIdsByUserNamesAsync(input.SpeakerUserNames)
            );
            
            await _eventRepository.UpdateAsync(@event);
        }

        [Authorize]
        public async Task UpdateSessionAsync(Guid id, Guid trackId, Guid sessionId, UpdateSessionDto input)
        {
            var @event = await _eventRepository.GetAsync(id);
            await CheckIfValidOwnerAsync(@event);
            
            @event.UpdateSession(
                trackId,
                sessionId,
                input.Title,
                input.Description,
                Clock.Normalize(input.StartTime),
                Clock.Normalize(input.EndTime),
                input.Language,
                await GetUserIdsByUserNamesAsync(input.SpeakerUserNames)
            );
            
            await _eventRepository.UpdateAsync(@event);
        }

        [Authorize]
        public async Task DeleteSessionAsync(Guid id, Guid trackId, Guid sessionId)
        {
            var @event = await _eventRepository.GetAsync(id, true);
            await CheckIfValidOwnerAsync(@event);

            @event.RemoveSession(trackId, sessionId);
            
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
        
        private async Task<List<Guid>> GetUserIdsByUserNamesAsync(List<string> userNames)
        {
            var userQueryable = await _userRepository.GetQueryableAsync();

            var query = userQueryable
                    .Where(u => userNames.Contains(u.UserName))
                    .Select(x => x.Id);
            
            var userIds = await AsyncExecuter.ToListAsync(query);
            if (userIds.Count != userNames.Count)
            {
                await CheckIfValidUserNamesAsync(userNames);
            }

            return userIds;
        }

        private async Task CheckIfValidOwnerAsync(Event @event)
        {
            var organization = await _organizationRepository.GetAsync(@event.OrganizationId);

            if (organization.OwnerUserId != CurrentUser.GetId())
            {
                throw new AbpAuthorizationException(
                    L["EventHub:NotAuthorizedToUpdateEvent", @event.Title],
                    EventHubErrorCodes.NotAuthorizedToUpdateEvent
                );
            }
        }
        
        private async Task CheckIfValidUserNamesAsync(List<string> speakerUserNames)
        {
            foreach (var userName in speakerUserNames)
            {
                var user = await _userRepository.AnyAsync(x => x.UserName == userName.Trim());
                if (!user)
                {
                    throw new BusinessException(EventHubErrorCodes.UserNotFound)
                        .WithData("UserName", userName);
                }
            }
        }

        private List<EventInListDto> GetEventInListDtoFromEventAndOrganizationTupleList(List<(Event @event, Organization organization)> items)
        {
            var now = Clock.Now;
            var events = items.Select(
                i =>
                {
                    var dto = ObjectMapper.Map<Event, EventInListDto>(i.@event);
                    dto.OrganizationName = i.organization.Name;
                    dto.OrganizationDisplayName = i.organization.DisplayName;
                    dto.IsLiveNow = i.@event.IsLive(now);
                    dto.Country = i.@event.CountryName;
                    return dto;
                }
            ).ToList();

            return events;
        }
    }
}
