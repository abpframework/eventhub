using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using EventHub.Events.Registrations;
using Microsoft.AspNetCore.Authorization;
using EventHub.Events;

namespace EventHub.Admin.Events.Registrations
{
    //[Authorize(EventHubPermissions.Events.Registrations.Default)]
    public class EventRegistrationAppService : EventHubAdminAppService, IEventRegistrationAppService
    {
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly EventRegistrationManager _eventRegistrationManager;


        public EventRegistrationAppService(
            IRepository<IdentityUser, Guid> userRepository, 
            IRepository<EventRegistration, Guid> eventRegistrationRepository,
            IRepository<Event, Guid> eventRepository,
            EventRegistrationManager eventRegistrationManager)
        {
            _userRepository = userRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _eventRepository = eventRepository;
            _eventRegistrationManager = eventRegistrationManager;
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

        //[Authorize(EventHubPermissions.Events.Registrations.RemoveAttendee)]
        public async Task RemoveAttendeeAsync(Guid eventId, Guid attendeeId)
        {
            await _eventRegistrationManager.UnregisterAsync(
                await _eventRepository.GetAsync(eventId),
                await _userRepository.GetAsync(attendeeId)
            );
        }

        //[Authorize(EventHubPermissions.Events.Registrations.AddAttendee)]
        public async Task RegisterUsersAsync(Guid eventId, List<Guid> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return;
            }

            var @event = await _eventRepository.GetAsync(eventId);

            var userQueryable = await _userRepository.GetQueryableAsync();
            var query = userQueryable.Where(user => userIds.Contains(user.Id));

            var users = await AsyncExecuter.ToListAsync(query);

            foreach (var user in users)
            {
                await _eventRegistrationManager.RegisterAsync(@event, user);
            }
        }
    }
}
