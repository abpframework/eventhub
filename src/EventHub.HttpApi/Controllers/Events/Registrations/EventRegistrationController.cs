using System;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace EventHub.Controllers.Events.Registrations
{
    [RemoteService(Name = EventHubRemoteServiceConsts.RemoteServiceName)]
    [Area("eventhub")]
    [ControllerName("EventRegistration")]
    [Route("api/eventhub/event-registration")]
    public class EventRegistrationController : EventHubController, IEventRegistrationAppService
    {
        private readonly IEventRegistrationAppService _eventRegistrationAppService;

        public EventRegistrationController(IEventRegistrationAppService eventRegistrationAppService)
        {
            _eventRegistrationAppService = eventRegistrationAppService;
        }

        [HttpGet]
        [Route("register/{eventId}")]
        public async Task RegisterAsync(Guid eventId)
        {
            await _eventRegistrationAppService.RegisterAsync(eventId);
        }

        [HttpGet]
        [Route("un-register/{eventId}")]
        public async Task UnregisterAsync(Guid eventId)
        {
            await _eventRegistrationAppService.UnregisterAsync(eventId);
        }

        [HttpGet]
        [Route("is-registered/{eventId}")]
        public async Task<bool> IsRegisteredAsync(Guid eventId)
        {
            return await _eventRegistrationAppService.IsRegisteredAsync(eventId);
        }

        [HttpGet]
        public async Task<PagedResultDto<EventAttendeeDto>> GetAttendeesAsync(Guid eventId)
        {
            return await _eventRegistrationAppService.GetAttendeesAsync(eventId);
        }

        [HttpGet]
        [Route("is-past-event/{eventId}")]
        public async Task<bool> IsPastEventAsync(Guid eventId)
        {
            return await _eventRegistrationAppService.IsPastEventAsync(eventId);
        }
    }
}
