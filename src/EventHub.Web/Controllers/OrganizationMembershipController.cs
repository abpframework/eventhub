using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Organizations.Memberships;
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
        
        [HttpGet]
        public async Task<IActionResult> GetList(OrganizationMemberListFilterDto input)
        {
            ViewData.Model = (await _organizationMembershipAppService.GetMembersAsync(input)).Items.ToList();
            
            return new PartialViewResult
            {
                ViewName = "~/Pages/Organizations/Components/MembersArea/_memberListSection.cshtml",
                ViewData = ViewData
            };
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
    }
}
