using System;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace EventHub.Web.Pages.Events.Components.AttendeesArea
{
    [Widget(
        AutoInitialize = true,
        RefreshUrl = "/Widgets/EventAttendeesArea"
    )]
    public class AttendeesAreaViewComponent : AbpViewComponent
    {
        private readonly IEventRegistrationAppService _eventRegistrationAppService;

        public AttendeesAreaViewComponent(IEventRegistrationAppService eventRegistrationAppService)
        {
            _eventRegistrationAppService = eventRegistrationAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid eventId)
        {
            var result = await _eventRegistrationAppService.GetAttendeesAsync(eventId);

            return View(
                "~/Pages/Events/Components/AttendeesArea/Default.cshtml",
                result
            );
        }
    }
}
