using System;
using System.Threading.Tasks;
using EventHub.Users;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Timing;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationManager : DomainService
    {
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly IClock _clock;

        public EventRegistrationManager(
            IRepository<EventRegistration, Guid> eventRegistrationRepository,
            IClock clock)
        {
            _eventRegistrationRepository = eventRegistrationRepository;
            _clock = clock;
        }

        public async Task RegisterAsync(
            Event @event,
            AppUser user)
        {
            CheckEventEndTime(@event);

            if (await IsRegisteredAsync(@event, user))
            {
                return;
            }

            await _eventRegistrationRepository.InsertAsync(
                new EventRegistration(
                    GuidGenerator.Create(),
                    @event.Id,
                    user.Id
                )
            );
        }

        public async Task UnregisterAsync(
            Event @event,
            AppUser user)
        {
            CheckEventEndTime(@event);

            await _eventRegistrationRepository.DeleteAsync(
                x => x.EventId == @event.Id && x.UserId == user.Id
            );
        }

        public async Task<bool> IsRegisteredAsync(
            Event @event,
            AppUser user)
        {
            return await _eventRegistrationRepository
                .AnyAsync(x => x.EventId == @event.Id && x.UserId == user.Id);
        }

        private void CheckEventEndTime(Event @event)
        {
            if (_clock.Now > @event.EndTime)
            {
                throw new BusinessException(EventHubErrorCodes.CantRegisterOrUnregisterForAPastEvent);
            }
        }
    }
}
