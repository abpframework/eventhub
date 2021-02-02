using System;
using System.Threading.Tasks;
using EventHub.Organizations.Memberships;
using EventHub.Web.Pages.Organizations.Components.JoinArea;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class OrganizationMembershipController : AbpController
    {
        private readonly IOrganizationMembershipAppService _organizationMembershipAppService;

        public OrganizationMembershipController(IOrganizationMembershipAppService organizationMembershipAppService)
        {
            _organizationMembershipAppService = organizationMembershipAppService;
        }
        
        [HttpPost]
        public async Task<NoContentResult> Join(Guid organizationId)
        {
            await _organizationMembershipAppService.JoinAsync(organizationId);
            return NoContent();
        }

        [HttpPost]
        public async Task<NoContentResult> Leave(Guid organizationId)
        {
            await _organizationMembershipAppService.LeaveAsync(organizationId);
            return NoContent();
        }
        
        [HttpGet]
        public IActionResult Widget(Guid organizationId)
        {
            return ViewComponent(
                typeof(JoinAreaViewComponent),
                new {organizationId}
            );
        }
    }
}