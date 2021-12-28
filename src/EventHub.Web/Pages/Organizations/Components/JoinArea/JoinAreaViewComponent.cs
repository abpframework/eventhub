using System;
using System.Threading.Tasks;
using EventHub.Organizations.Memberships;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Users;

namespace EventHub.Web.Pages.Organizations.Components.JoinArea
{
    [Widget(
        AutoInitialize = true,
        RefreshUrl = "/Widgets/JoinArea",
        ScriptFiles = new[] {"/Pages/Organizations/Components/JoinArea/join-area.js"}
    )]
    public class JoinAreaViewComponent : AbpViewComponent
    {
        private readonly IOrganizationMembershipAppService _organizationMembershipAppService;
        private readonly ICurrentUser _currentUser;

        public JoinAreaViewComponent(
            IOrganizationMembershipAppService organizationMembershipAppService, 
            ICurrentUser currentUser)
        {
            _organizationMembershipAppService = organizationMembershipAppService;
            _currentUser = currentUser;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(Guid organizationId)
        {
            var model = new MembershipAreaViewComponentModel
            {
                OrganizationId = organizationId,
                IsLoggedIn = _currentUser.IsAuthenticated
            };

            if (model.IsLoggedIn)
            {
                model.IsJoined = await _organizationMembershipAppService.IsJoinedAsync(organizationId);
            }

            return View("~/Pages/Organizations/Components/JoinArea/Default.cshtml", model);
        }

        public class MembershipAreaViewComponentModel
        {
            public Guid OrganizationId { get; set; }
            public bool IsLoggedIn { get; set; }
            public bool IsJoined { get; set; }
        }
    }
}
