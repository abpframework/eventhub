using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace EventHub.Web.Pages.Events.Components.ListArea
{
    [Widget(AutoInitialize = true)]
    public class ListAreaViewComponent : AbpViewComponent
    {
        private readonly IEventAppService _eventAppService;

        public ListAreaViewComponent(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            Guid? organizationId,
            DateTime? minDate, 
            DateTime? maxDate, 
            bool? isOnline, 
            int? skipCount, 
            int maxResultCount = 15)
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
                "~/Pages/Events/Components/ListArea/Default.cshtml",
                new ListAreaViewComponentModel
                {
                   Events = result.Items,
                   TotalCount = result.TotalCount
                }
            );
        }
        
        public class ListAreaViewComponentModel
        {
            public IReadOnlyList<EventInListDto> Events { get; set; }
            
            public long TotalCount { get; set; }
        }
    }
}