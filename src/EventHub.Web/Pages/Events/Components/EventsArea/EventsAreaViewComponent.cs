using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace EventHub.Web.Pages.Events.Components.EventsArea
{
    [Widget(
        AutoInitialize = true,
        ScriptFiles = new[] {"/Pages/Events/Components/EventsArea/events-area.js"})]
    public class EventsAreaViewComponent : AbpViewComponent
    {
        private readonly IEventAppService _eventAppService;

        public List<EventInListDto> Events { get; set; }
        
        public EventsAreaViewComponent(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            Guid? organizationId,
            DateTime? minDate, 
            DateTime? maxDate,
            bool? isOnline,
            int? skipCount,
            int maxResultCount = 15,
            bool isPagination = true)
        {
            var result = await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    OrganizationId = organizationId,
                    MinDate = minDate,
                    MaxDate = maxDate,
                    IsOnline = isOnline,
                    SkipCount = skipCount.GetValueOrDefault(),
                    MaxResultCount = maxResultCount
                }
            );
            
            return View(
                "~/Pages/Events/Components/EventsArea/Default.cshtml",
                new ListAreaViewComponentModel
                {
                   Events = result.Items,
                   TotalCount = result.TotalCount,
                   OrganizationId = organizationId,
                   MinDate = minDate,
                   MaxDate = maxDate,
                   IsOnline = isOnline,
                   SkipCount = skipCount.GetValueOrDefault(),
                   MaxResultCount = maxResultCount,
                   IsPagination = isPagination
                }
            );
        }
        
        public class ListAreaViewComponentModel
        {
            public IReadOnlyList<EventInListDto> Events { get; set; }
            
            public long TotalCount { get; set; }

            public Guid? OrganizationId { get; set; }

            public DateTime? MinDate { get; set; }

            public DateTime? MaxDate { get; set; }

            public bool? IsOnline { get; set; }
            
            public int SkipCount { get; set; }
            
            public int MaxResultCount { get; set; }

            public bool IsPagination { get; set; }
        }
    }
}