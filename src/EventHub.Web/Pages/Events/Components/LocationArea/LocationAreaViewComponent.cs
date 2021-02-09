using System;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Events.Registrations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Users;

namespace EventHub.Web.Pages.Events.Components.LocationArea
{
    [Widget(
        AutoInitialize = true,
        RefreshUrl = "/Widgets/EventLocationArea",
        ScriptFiles = new[] {"/Pages/Events/Components/LocationArea/location-area.js"}
    )]
    public class LocationAreaViewComponent : AbpViewComponent
    {
        private readonly IEventAppService _eventAppService;
        private readonly IEventRegistrationAppService _eventRegistrationAppService;
        private readonly ICurrentUser _currentUser;

        public LocationAreaViewComponent(
            IEventAppService eventAppService, 
            IEventRegistrationAppService eventRegistrationAppService,
            ICurrentUser currentUser)
        {
            _eventAppService = eventAppService;
            _eventRegistrationAppService = eventRegistrationAppService;
            _currentUser = currentUser;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(Guid eventId)
        {
            var isLoggedIn = _currentUser.IsAuthenticated;
            
            var model = new LocationAreaViewComponentModel
            {
                EventId = eventId,
                IsRegistered = false
            };
            
            if (isLoggedIn)
            {
                var @event = await _eventAppService.GetLocationAsync(eventId);
                
                model.IsOnline = @event.IsOnline;
                model.IsRegistered = @event.IsRegistered;
                model.OnlineLink = @event.OnlineLink;
                model.Country = @event.Country;
                model.City = @event.City;
            }

            return View("~/Pages/Events/Components/LocationArea/Default.cshtml", model);
        }
        
        public class LocationAreaViewComponentModel
        {
            public Guid EventId { get; set; }
            public bool IsRegistered { get; set; }
            public bool IsOnline { get; set; }
            public string OnlineLink { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
        }
    }
}