using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
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
    }
}