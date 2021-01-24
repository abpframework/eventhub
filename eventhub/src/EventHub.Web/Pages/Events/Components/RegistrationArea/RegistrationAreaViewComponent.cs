using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace EventHub.Web.Pages.Events.Components.RegistrationArea
{
    [Widget(
        AutoInitialize = true,
        RefreshUrl = "/Widgets/EventRegistrationArea",
        ScriptFiles = new[] {"/Pages/Events/Components/RegistrationArea/registration-area.js"}
    )]
    public class RegistrationAreaViewComponent : AbpViewComponent
    {
        public RegistrationAreaViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid eventId)
        {
            return View();
        }
    }
}
