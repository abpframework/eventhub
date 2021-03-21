using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;

        public EventManager(
            EventUrlCodeGenerator eventUrlCodeGenerator, 
            IRepository<Event, Guid> eventRepository,
            IRepository<EventRegistration, Guid> eventRegistrationRepository
            )
        {
            _eventUrlCodeGenerator = eventUrlCodeGenerator;
            _eventRepository = eventRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
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
                    throw new BusinessException(EventHubErrorCodes.CapacityCantBeLowerThanRegisteredUserCount);
                }
            }

            @event.Capacity = capacity;
        }
    }
}
