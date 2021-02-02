using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<IViewComponentResult> InvokeAsync(Guid organizationId)
        {
            var result = await _organizationMembershipAppService.GetMembersAsync(organizationId);

            return View(
               "~/Pages/Organizations/Components/MembersArea/Default.cshtml",
                new MembersAreaViewComponentModel
                {
                    OrganizationId = organizationId,
                    Members = result.Items,
                    TotalCount = result.TotalCount
                }
            );
        }
        
        public class MembersAreaViewComponentModel
        {
            public IReadOnlyList<OrganizationMemberDto> Members { get; set; }

            public long TotalCount { get; set; }

            public Guid OrganizationId { get; set; }

            public string GetMemberName(OrganizationMemberDto member)
            {
                var nameBuilder = new StringBuilder();

                if (!member.Name.IsNullOrEmpty())
                {
                    nameBuilder.Append(member.Name);
                }

                if (!member.Surname.IsNullOrEmpty())
                {
                    nameBuilder.Append(member.Surname);
                }

                if (nameBuilder.Length == 0)
                {
                    nameBuilder.Append(member.UserName);
                }

                return nameBuilder.ToString();
            }
        }
    }
}