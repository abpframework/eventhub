using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Organizations.Memberships;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace EventHub.Web.Pages.Organizations.Components.MembersArea
{
    [Widget(
        AutoInitialize = true,
        RefreshUrl = "/Widgets/OrganizationMembersArea",
        ScriptFiles = new[] {"/Pages/Organizations/Components/MembersArea/members-area.js"},
        StyleFiles = new[] {"/Pages/Organizations/Components/MembersArea/members-area.css"}
    )]
    public class MembersAreaViewComponent : AbpViewComponent
    {
        private readonly IOrganizationMembershipAppService _organizationMembershipAppService;
        
        public MembersAreaViewComponent(IOrganizationMembershipAppService organizationMembershipAppService)
        {
            _organizationMembershipAppService = organizationMembershipAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            Guid organizationId,
            int? skipCount,
            int maxResultCount = 26,
            bool isPagination = true,
            bool isMoreDetail = false)
        {
            var result = await _organizationMembershipAppService.GetMembersAsync(new OrganizationMemberListFilterDto
            {
                OrganizationId = organizationId,
                SkipCount = skipCount.GetValueOrDefault(),
                MaxResultCount = maxResultCount
            });

            return View(
               "~/Pages/Organizations/Components/MembersArea/Default.cshtml",
                new MembersAreaViewComponentModel
                {
                    OrganizationId = organizationId,
                    Members = result.Items,
                    TotalCount = result.TotalCount,
                    SkipCount = skipCount.GetValueOrDefault(),
                    MaxResultCount = maxResultCount,
                    IsPagination = isPagination,
                    IsMoreDetail = isMoreDetail
                }
            );
        }
        
        public class MembersAreaViewComponentModel
        {
            public IReadOnlyList<OrganizationMemberDto> Members { get; set; }

            public long TotalCount { get; set; }

            public Guid OrganizationId { get; set; }
            
            public int SkipCount { get; set; }
            
            public int MaxResultCount { get; set; }

            public bool IsPagination { get; set; }
            
            public bool IsMoreDetail { get; set; }
        }
    }
}