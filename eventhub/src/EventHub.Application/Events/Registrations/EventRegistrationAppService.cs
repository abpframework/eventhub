using System;
using System.Threading.Tasks;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationAppService : EventHubAppService, IEventRegistrationAppService
    {
        private readonly EventRegistrationManager _eventRegistrationManager;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IRepository<Event, Guid> _eventRepository;

        public EventRegistrationAppService(
            EventRegistrationManager eventRegistrationManager,
            IRepository<AppUser, Guid> userRepository,
            IRepository<Event, Guid> eventRepository)
        {
            _eventRegistrationManager = eventRegistrationManager;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        [Authorize]
        public async Task RegisterAsync(Guid id)
        {
            await _eventRegistrationManager.RegisterAsync(
                await _eventRepository.GetAsync(id),
                await _userRepository.GetAsync(CurrentUser.GetId())
            );
        }

        [Authorize]
        public async Task UnregisterAsync(Guid id)
        {
            await _eventRegistrationManager.UnregisterAsync(
                await _eventRepository.GetAsync(id),
                await _userRepository.GetAsync(CurrentUser.GetId())
            );
        }
    }
}
