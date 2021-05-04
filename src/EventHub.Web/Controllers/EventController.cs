using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Web.Pages.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using EditPageModel = EventHub.Web.Pages.Events.EditPageModel;

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
        
        [HttpGet]
        [Route("get-list")]
        public async Task<IActionResult> GetList(EventListFilterDto input)
        {
            ViewData.Model = (await _eventAppService.GetListAsync(input)).Items.ToList();
            
            return new PartialViewResult
            {
                ViewName = "~/Pages/Events/Components/EventsArea/_eventListSection.cshtml",
                ViewData = ViewData
            };
        }
        
        [Authorize]
        [HttpPost]
        [Route("update-timing")]
        public async Task UpdateEventTiming(EditPageModel.EditEventTimingViewModel input)
        {
            await _eventAppService.UpdateEventTimingAsync(input.Id, new UpdateEventTimingDto { StartTime = input.StartTime, EndTime = input.EndTime });
        }
        
        [HttpPost]
        [Authorize]
        [Route("save-cover-image")]
        public async Task SaveCoverImage([FromForm] EventCoverImageInput input)
        {
            var coverImageContent = new byte[] {};
            
            if (input.CoverImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await input.CoverImageFile.CopyToAsync(memoryStream);
                    coverImageContent = memoryStream.ToArray();
                }
                
                await _eventAppService.SaveCoverImageAsync(input.EventId, coverImageContent);
            }
        }
    }
}