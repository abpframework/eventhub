using System;
using System.Collections.Generic;
using System.Linq;
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
        public Task<PagedResultDto<EventAttendeeDto>> GetAttendeesAsync(Guid eventId)
        {
            return _eventRegistrationAppService.GetAttendeesAsync(eventId);
        }

        [HttpPost]
        public Task RegisterUsersAsync(Guid eventId, List<Guid> userIds)
        {
            return _eventRegistrationAppService.RegisterUsersAsync(eventId, userIds);
        }

        [HttpGet]
        public Task RemoveAttendeeAsync(Guid eventId, Guid attendeeId)
        {
            return _eventRegistrationAppService.RemoveAttendeeAsync(eventId, attendeeId);
        }
    }
}
