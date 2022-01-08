using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationAppService : EventHubAppService, IEventRegistrationAppService
    {
        private readonly EventRegistrationManager _eventRegistrationManager;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly EventRegistrationNotifier _eventRegistrationNotifier;

        public EventRegistrationAppService(
            EventRegistrationManager eventRegistrationManager,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository, 
            EventRegistrationNotifier eventRegistrationNotifier)
        {
            _eventRegistrationManager = eventRegistrationManager;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _eventRegistrationNotifier = eventRegistrationNotifier;
        }

        [Authorize]
        public async Task RegisterAsync(Guid eventId)
        {
            var @event = await _eventRepository.GetAsync(eventId);
            var user = await _userRepository.GetAsync(CurrentUser.GetId());
            
            await _eventRegistrationManager.RegisterAsync(@event, user);
            
            await _eventRegistrationNotifier.NotifyAsync(@event, user);
        }

        [Authorize]
        public async Task UnregisterAsync(Guid eventId)
        {
            await _eventRegistrationManager.UnregisterAsync(
                await _eventRepository.GetAsync(eventId),
                await _userRepository.GetAsync(CurrentUser.GetId())
            );
        }

        [Authorize]
        public async Task<bool> IsRegisteredAsync(Guid eventId)
        {
            return await _eventRegistrationManager.IsRegisteredAsync(
                await _eventRepository.GetAsync(eventId),
                await _userRepository.GetAsync(CurrentUser.GetId())
            );
        }

        public async Task<PagedResultDto<EventAttendeeDto>> GetAttendeesAsync(Guid eventId)
        {
            var eventRegistrationQueryable = await _eventRegistrationRepository.GetQueryableAsync();
            var userQueryable = await _userRepository.GetQueryableAsync();

            var query = from eventRegistration in eventRegistrationQueryable
                join user in userQueryable on eventRegistration.UserId equals user.Id
                where eventRegistration.EventId == eventId
                orderby eventRegistration.CreationTime descending
                select user;

            var totalCount = await AsyncExecuter.CountAsync(query);
            var users = await AsyncExecuter.ToListAsync(query.Take(10));

            return new PagedResultDto<EventAttendeeDto>(
                totalCount,
                ObjectMapper.Map<List<IdentityUser>, List<EventAttendeeDto>>(users)
            );
        }

        public async Task<bool> IsPastEventAsync(Guid eventId)
        {
            var @event = await _eventRepository.GetAsync(eventId);

            return _eventRegistrationManager.IsPastEvent(@event);
        }
    }
}
