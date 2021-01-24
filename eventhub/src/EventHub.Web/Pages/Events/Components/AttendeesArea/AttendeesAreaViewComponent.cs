using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace EventHub.Web.Pages.Events.Components.AttendeesArea
{
    [Widget(
        AutoInitialize = true,
        RefreshUrl = "/Widgets/EventAttendeesArea",
        ScriptFiles = new[] {"/Pages/Events/Components/AttendeesArea/attendees-area.js"},
        StyleFiles = new[] {"/Pages/Events/Components/AttendeesArea/attendees-area.css"}
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
                new AttendeesAreaViewComponentModel
                {
                    EventId = eventId,
                    Attendees = result.Items,
                    TotalCount = result.TotalCount
                }
            );
        }

        public class AttendeesAreaViewComponentModel
        {
            public IReadOnlyList<EventAttendeeDto> Attendees { get; set; }

            public long TotalCount { get; set; }

            public Guid EventId { get; set; }

            public string GetAttendeeName(EventAttendeeDto attendee)
            {
                var nameBuilder = new StringBuilder();

                if (!attendee.Name.IsNullOrEmpty())
                {
                    nameBuilder.Append(attendee.Name);
                }

                if (!attendee.Surname.IsNullOrEmpty())
                {
                    nameBuilder.Append(attendee.Surname);
                }

                if (nameBuilder.Length == 0)
                {
                    nameBuilder.Append(attendee.UserName);
                }

                return nameBuilder.ToString();
            }
        }
    }
}
