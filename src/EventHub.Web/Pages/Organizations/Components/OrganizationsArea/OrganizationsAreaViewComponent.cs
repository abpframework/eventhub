using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace EventHub.Web.Pages.Organizations.Components.OrganizationsArea
{
    [Widget(
        AutoInitialize = true,
        ScriptFiles = new[] {"/Pages/Organizations/Components/OrganizationsArea/organizations-area.js"})]
    public class OrganizationsAreaViewComponent : AbpViewComponent
    {
        private readonly IOrganizationAppService _organizationAppService;

        public OrganizationsAreaViewComponent(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            int? skipCount,
            int maxResultCount = 15,
            bool isPagination = true)
        {
            var result = await _organizationAppService.GetListAsync(
                new PagedResultRequestDto
                {
                    SkipCount = skipCount.GetValueOrDefault(),
                    MaxResultCount = maxResultCount
                }
            );

            return View(
                "~/Pages/Organizations/Components/OrganizationsArea/Default.cshtml",
                new OrganizationsAreaViewComponent.ListAreaViewComponentModel
                {
                    Organizations = result.Items,
                    TotalCount = result.TotalCount,
                    SkipCount = skipCount.GetValueOrDefault(),
                    MaxResultCount = maxResultCount,
                    IsPagination = isPagination
                }
            );
        }
        
        public class ListAreaViewComponentModel
        {
            public IReadOnlyList<OrganizationInListDto> Organizations { get; set; }
            
            public long TotalCount { get; set; }

            public int SkipCount { get; set; }
            
            public int MaxResultCount { get; set; }

            public bool IsPagination { get; set; }
        }
    }
}