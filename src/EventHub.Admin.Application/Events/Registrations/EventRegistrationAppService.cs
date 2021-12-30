using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Events.Registrations
{
    [Authorize(EventHubPermissions.Events.Registrations.Default)]
    public class EventRegistrationAppService : EventHubAdminAppService, IEventRegistrationAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly EventRegistrationManager _eventRegistrationManager;

        public EventRegistrationAppService(
            IUserRepository userRepository, 
            IEventRegistrationRepository eventRegistrationRepository,
            IEventRepository eventRepository,
            EventRegistrationManager eventRegistrationManager)
        {
            _userRepository = userRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _eventRepository = eventRepository;
            _eventRegistrationManager = eventRegistrationManager;
        }

        public async Task<PagedResultDto<EventAttendeeDto>> GetAttendeesAsync(GetEventRegistrationListInput input)
        {
            var totalCount = await _eventRegistrationRepository.GetCountAsync(input.EventId);
            var items = await _eventRegistrationRepository.GetListAsync(input.EventId, input.Sorting, input.SkipCount, input.MaxResultCount);
            var users = ObjectMapper.Map<List<EventRegistrationWithDetails>, List<EventAttendeeDto>>(items);

            return new PagedResultDto<EventAttendeeDto>(totalCount, users);
        }

        [Authorize(EventHubPermissions.Events.Registrations.RemoveAttendee)]
        public async Task UnRegisterAttendeeAsync(Guid eventId, Guid attendeeId)
        {
            await _eventRegistrationRepository.DeleteAsync(x => x.EventId == eventId && x.UserId == attendeeId);
        }

        [Authorize(EventHubPermissions.Events.Registrations.AddAttendee)]
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

        public async Task<List<Guid>> GetAllAttendeeIdsAsync(Guid eventId)
        {
            var eventRegistrationQueryable = await _eventRegistrationRepository.GetQueryableAsync();
            var userQueryable = await _userRepository.GetQueryableAsync();

            var query = from eventRegistration in eventRegistrationQueryable
                join user in userQueryable on eventRegistration.UserId equals user.Id
                where eventRegistration.EventId == eventId
                orderby eventRegistration.CreationTime descending
                select user.Id;

            return await AsyncExecuter.ToListAsync(query);
        }
    }
}
