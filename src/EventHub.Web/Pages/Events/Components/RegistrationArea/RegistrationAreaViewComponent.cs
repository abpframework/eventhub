using System;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Users;

namespace EventHub.Web.Pages.Events.Components.RegistrationArea
{
    [Widget(
        AutoInitialize = true,
        RefreshUrl = "/Widgets/RegistrationArea",
        ScriptFiles = new[] {"/Pages/Events/Components/RegistrationArea/registration-area.js"}
    )]
    public class RegistrationAreaViewComponent : AbpViewComponent
    {
        private readonly IEventRegistrationAppService _eventRegistrationAppService;
        private readonly ICurrentUser _currentUser;

        public RegistrationAreaViewComponent(
            IEventRegistrationAppService eventRegistrationAppService,
            ICurrentUser currentUser)
        {
            _eventRegistrationAppService = eventRegistrationAppService;
            _currentUser = currentUser;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid eventId)
        {
            var model = new RegistrationAreaViewComponentModel
            {
                EventId = eventId,
                IsLoggedIn = _currentUser.IsAuthenticated
            };

            if (model.IsLoggedIn)
            {
                model.IsRegistered = await _eventRegistrationAppService.IsRegisteredAsync(eventId);
                model.IsPastEvent = await _eventRegistrationAppService.IsPastEventAsync(eventId);
            }

            return View("~/Pages/Events/Components/RegistrationArea/Default.cshtml", model);
        }

        public class RegistrationAreaViewComponentModel
        {
            public Guid EventId { get; set; }
            public bool IsLoggedIn { get; set; }
            public bool IsRegistered { get; set; }
            public bool IsPastEvent { get; set; }
        }
    }
}
