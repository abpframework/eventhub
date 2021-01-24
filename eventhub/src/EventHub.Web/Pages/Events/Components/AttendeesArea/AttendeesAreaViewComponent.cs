using System;
using System.Threading.Tasks;
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
        public AttendeesAreaViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid eventId)
        {
            return View("~/Pages/Events/Components/AttendeesArea/Default.cshtml");
        }
    }
}
