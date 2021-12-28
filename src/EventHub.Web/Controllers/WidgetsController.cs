using System;
using EventHub.Web.Pages.Events.Components.AttendeesArea;
using EventHub.Web.Pages.Events.Components.CreateOrEditEventArea;
using EventHub.Web.Pages.Events.Components.LocationArea;
using EventHub.Web.Pages.Events.Components.RegistrationArea;
using EventHub.Web.Pages.Organizations.Components.JoinArea;
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
        
        [HttpGet]
        public IActionResult RegistrationArea(Guid eventId)
        {
            return ViewComponent(
                typeof(RegistrationAreaViewComponent),
                new {eventId}
            );
        }
        
        [HttpGet]
        public IActionResult JoinArea(Guid organizationId)
        {
            return ViewComponent(
                typeof(JoinAreaViewComponent),
                new {organizationId}
            );
        }
        
        [HttpGet]
        public IActionResult CreateOrEditEventArea(string eventUrlCode, CreateOrEditEventAreaViewComponent.ProgressStepType stepType)
        {
            return ViewComponent(
                typeof(CreateOrEditEventAreaViewComponent),
                new
                {
                    eventUrlCode, 
                    stepType
                }
            );
        }
    }
}
