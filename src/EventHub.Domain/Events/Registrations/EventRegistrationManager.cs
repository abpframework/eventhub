using System.Threading.Tasks;
using EventHub.Organizations.Plans;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Timing;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationManager : DomainService
    {
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IClock _clock;
        private readonly PlanFeatureManager _planFeatureManager;

        public EventRegistrationManager(
            IEventRegistrationRepository eventRegistrationRepository,
            IGuidGenerator guidGenerator,
            IClock clock, 
            PlanFeatureManager planFeatureManager)
        {
            _eventRegistrationRepository = eventRegistrationRepository;
            _guidGenerator = guidGenerator;
            _clock = clock;
            _planFeatureManager = planFeatureManager;
        }

        public async Task RegisterAsync(
            Event @event,
            IdentityUser user)
        {
            if (IsPastEvent(@event))
            {
                throw new BusinessException(EventHubErrorCodes.CantRegisterOrUnregisterForAPastEvent);
            }

            if (await IsRegisteredAsync(@event, user))
            {
                return;
            }
            
            if (@event.Capacity != null)
            {
                var registrationCount = await _eventRegistrationRepository.CountAsync(x => x.EventId == @event.Id);
                if (registrationCount >= @event.Capacity)
                {
                    throw new BusinessException(EventHubErrorCodes.CapacityOfEventFull)
                        .WithData("EventTitle", @event.Title);
                }
            }

            if (!await _planFeatureManager.CanRegisterToEventAsync(@event))
            {
                throw new BusinessException(EventHubErrorCodes.CannotRegisterToEvent);
            }
                
            await _eventRegistrationRepository.InsertAsync(
                new EventRegistration(
                    _guidGenerator.Create(),
                    @event.Id,
                    user.Id
                )
            );
        }

        public async Task UnregisterAsync(
            Event @event,
            IdentityUser user)
        {
            if (IsPastEvent(@event))
            {
                throw new BusinessException(EventHubErrorCodes.CantRegisterOrUnregisterForAPastEvent);
            }

            await _eventRegistrationRepository.DeleteAsync(
                x => x.EventId == @event.Id && x.UserId == user.Id
            );
        }

        public async Task<bool> IsRegisteredAsync(
            Event @event,
            IdentityUser user)
        {
            return await _eventRegistrationRepository
                .ExistsAsync(@event.Id, user.Id);
        }

        public bool IsPastEvent(Event @event)
        {
            return _clock.Now > @event.EndTime;
        }
    }
}
