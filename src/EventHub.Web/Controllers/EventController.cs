using System;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Web.Pages.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    [Route("api/event")]
    public class EventController : AbpController
    {
        private readonly IEventAppService _eventAppService;

        public EventController(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }
        
        [Authorize]
        [HttpPost]
        [Route("update-timing")]
        public async Task UpdateEventTiming(EditPageModel.EditEventTimingViewModel input)
        {
            await _eventAppService.UpdateEventTimingAsync(input.Id, new UpdateEventTimingDto { StartTime = input.StartTime, EndTime = input.EndTime });
        }
    }
}