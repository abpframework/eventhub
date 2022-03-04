using System;
using System.Threading.Tasks;
using EventHub.Countries;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using EventHub.Organizations.Plans;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace EventHub.Events
{
    public class EventManager : DomainService
    {
        private readonly EventUrlCodeGenerator _eventUrlCodeGenerator;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly IRepository<Country, Guid> _countriesRepository;
        private readonly PlanFeatureManager _planFeatureManager;
        public EventManager(
            EventUrlCodeGenerator eventUrlCodeGenerator, 
            IRepository<EventRegistration, Guid> eventRegistrationRepository, 
            IRepository<Country, Guid> countriesRepository, 
            PlanFeatureManager planFeatureManager)
        {
            _eventUrlCodeGenerator = eventUrlCodeGenerator;
            _eventRegistrationRepository = eventRegistrationRepository;
            _countriesRepository = countriesRepository;
            _planFeatureManager = planFeatureManager;
        }

        public async Task<Event> CreateAsync(
            Organization organization,
            string title,
            DateTime startTime,
            DateTime endTime,
            string description)
        {
            if (!await _planFeatureManager.CanCreateNewEventAsync(organization.Id))
            {
                throw new BusinessException(EventHubErrorCodes.CannotCreateNewEvent);
            }
            
            return new Event(
                GuidGenerator.Create(),
                organization.Id,
                await _eventUrlCodeGenerator.GenerateAsync(),
                title,
                startTime,
                endTime,
                description
            );
        }
        
        public async Task AddTrackAsync(Event @event, string name)
        {
            if (!await _planFeatureManager.CanAddNewTrackAsync(@event))
            {
                throw new BusinessException(EventHubErrorCodes.CannotAddNewTrack);
            }
            
            @event.AddTrack(
                GuidGenerator.Create(),
                name
            );
        }

        public async Task SetCapacityAsync(
            Event @event,
            int? capacity)
        {
            if (capacity.HasValue)
            {
                var registeredUserCount = await _eventRegistrationRepository.CountAsync(x => x.EventId == @event.Id);
                if (capacity.Value < registeredUserCount)
                {
                    throw new BusinessException(EventHubErrorCodes.CapacityCanNotBeLowerThanRegisteredUserCount);
                }
            }

            @event.Capacity = capacity;
        }
        
        public async Task SetLocationAsync(
            Event @event,
            bool isOnline,
            string onlineLink,
            Guid? countryId,
            string city)
        {
            @event.IsOnline = isOnline;
            
            if (@event.IsOnline)
            {
                @event.OnlineLink = onlineLink;
                @event.CountryId = null;
                @event.CountryName = null;
                @event.City = null;
            }
            else
            {
                Check.NotNull(countryId, nameof(countryId));
                Check.NotNull(city, nameof(city));

                @event.OnlineLink = null;
                @event.CountryId = countryId;
                @event.CountryName = (await _countriesRepository.GetAsync(@event.CountryId!.Value)).Name;
                @event.City = city;
            }
        }
    }
}
