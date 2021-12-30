using System;
using System.Threading.Tasks;
using EventHub.Countries;
using EventHub.Events.Registrations;
using EventHub.Organizations;
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

        public EventManager(
            EventUrlCodeGenerator eventUrlCodeGenerator, 
            IRepository<EventRegistration, Guid> eventRegistrationRepository, 
            IRepository<Country, Guid> countriesRepository)
        {
            _eventUrlCodeGenerator = eventUrlCodeGenerator;
            _eventRegistrationRepository = eventRegistrationRepository;
            _countriesRepository = countriesRepository;
        }

        public async Task<Event> CreateAsync(
            Organization organization,
            string title,
            DateTime startTime,
            DateTime endTime,
            string description)
        {
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
