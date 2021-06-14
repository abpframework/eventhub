using System;
using EventHub.Web.Pages.Events.Components.AttendeesArea;
using EventHub.Web.Pages.Events.Components.LocationArea;
using EventHub.Web.Pages.Organizations.Components.MembersArea;
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
        
        [HttpGet]
        public IActionResult OrganizationMembersArea(
            Guid organizationId,
            int? skipCount,
            int maxResultCount,
            bool isPagination,
            bool isMoreDetail)
        {
            return ViewComponent(
                typeof(MembersAreaViewComponent),
                new
                {
                    organizationId,
                    skipCount,
                    maxResultCount,
                    isPagination,
                    isMoreDetail
                }
            );
        }
        
        [HttpGet]
        public IActionResult EventLocationArea(Guid eventId)
        {
            return ViewComponent(
                typeof(LocationAreaViewComponent),
                new {eventId}
            );
        }
    }
}
