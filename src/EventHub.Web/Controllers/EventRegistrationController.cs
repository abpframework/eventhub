using System;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class EventRegistrationController : AbpController
    {
        private readonly IEventRegistrationAppService _eventRegistrationAppService;

        public EventRegistrationController(IEventRegistrationAppService eventRegistrationAppService)
        {
            _eventRegistrationAppService = eventRegistrationAppService;
        }

        [HttpPost]
        public async Task<NoContentResult> Register(Guid eventId)
        {
            await _eventRegistrationAppService.RegisterAsync(eventId);
            return NoContent();
        }

        [HttpPost]
        public async Task<NoContentResult> Unregister(Guid eventId)
        {
            await _eventRegistrationAppService.UnregisterAsync(eventId);
            return NoContent();
        }
    }
}
