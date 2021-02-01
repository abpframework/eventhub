using System;
using EventHub.Web.Pages.Events.Components.AttendeesArea;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class WidgetsController : AbpController
    {
        [HttpGet]
        public IActionResult EventAttendeesArea(Guid eventId)
        {
            return ViewComponent(
                typeof(AttendeesAreaViewComponent),
                new {eventId}
            );
        }
    }
}
