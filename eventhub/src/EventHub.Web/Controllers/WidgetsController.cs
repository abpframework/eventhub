using System;
using EventHub.Web.Pages.Events.Components.AttendeesArea;
using EventHub.Web.Pages.Events.Components.RegistrationArea;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class WidgetsController : AbpController
    {
        [HttpGet]
        public IActionResult EventRegistrationArea(Guid eventId)
        {
            return ViewComponent(
                typeof(RegistrationAreaViewComponent),
                new {eventId}
            );
        }

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
