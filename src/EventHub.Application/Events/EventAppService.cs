using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Countries;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace EventHub.Events
{
    public class EventAppService : EventHubAppService, IEventAppService
    {
        private readonly EventManager _eventManager;
        private readonly EventRegistrationManager _eventRegistrationManager;
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IRepository<Country, Guid> _countriesRepository;

        public EventAppService(
            EventManager eventManager,
            EventRegistrationManager eventRegistrationManager,
            IRepository<Event, Guid> eventRepository,
            IRepository<Organization, Guid> organizationRepository, 
            IRepository<AppUser, Guid> userRepository, 
            IRepository<Country, Guid> countriesRepository)
        {
            _eventManager = eventManager;
            _eventRegistrationManager = eventRegistrationManager;
            _eventRepository = eventRepository;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _countriesRepository = countriesRepository;
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

            @event.SetLocation(input.IsOnline, input.Link, input.CountryId, input.City);
            @event.Capacity = input.Capacity;

            await _eventRepository.InsertAsync(@event);

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

        [Authorize]
        public async Task<EventLocationDto> GetLocationAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            var user = await _userRepository.GetAsync(CurrentUser.GetId());

            var dto = ObjectMapper.Map<Event, EventLocationDto>(@event);
            
            dto.IsRegistered = await _eventRegistrationManager.IsRegisteredAsync(@event, user);
            
            if (!dto.IsRegistered)
            {
                dto.Link = null;
                dto.City = null;
            }
             
            if (dto.IsRegistered && @event.CountryId.HasValue)
            {
                dto.Country = (await _countriesRepository.GetAsync(@event.CountryId.Value)).Name;
            }

            return dto;
        }
        
        [Authorize]
        public async Task<List<CountryLookupDto>> GetCountriesLookupAsync()
        {
            var countriesQueryable = await _countriesRepository.GetQueryableAsync();

            var query = from country in countriesQueryable
                orderby country.Name ascending
                select country;

            var countries = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Country>, List<CountryLookupDto>>(countries);
        }
    }
}