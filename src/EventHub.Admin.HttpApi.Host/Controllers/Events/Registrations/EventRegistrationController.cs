using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Admin.Events.Registrations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Admin.Controllers.Events.Registrations
{
    [RemoteService(Name = EventHubAdminRemoteServiceConsts.RemoteServiceName)]
    [Controller]
    [Area("eventhub-admin")]
    [ControllerName("EventRegistration")]
    [Route("/api/eventhub/admin/event-registration")]
    public class EventRegistrationController : AbpController, IEventRegistrationAppService
    {
        private readonly IEventRegistrationAppService _eventRegistrationAppService;

        public EventRegistrationController(IEventRegistrationAppService eventRegistrationAppService)
        {
            _eventRegistrationAppService = eventRegistrationAppService;
        }

        [HttpGet("{eventId}")]
        public Task<List<Guid>> GetAllAttendeeIdsAsync(Guid eventId)
        {
            return _eventRegistrationAppService.GetAllAttendeeIdsAsync(eventId);
        }

        [HttpGet]
        public Task<PagedResultDto<EventAttendeeDto>> GetAttendeesAsync(GetEventRegistrationListInput input)
        {
            return _eventRegistrationAppService.GetAttendeesAsync(input);
        }

        [HttpPost]
        public Task RegisterUsersAsync(Guid eventId, List<Guid> userIds)
        {
            return _eventRegistrationAppService.RegisterUsersAsync(eventId, userIds);
        }

        [HttpDelete]
        public Task UnRegisterAttendeeAsync(Guid eventId, Guid attendeeId)
        {
            return _eventRegistrationAppService.UnRegisterAttendeeAsync(eventId, attendeeId);
        }
    }
}
