using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Pages.Organizations
{
    public class ProfilePageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        public OrganizationProfileDto Organization { get; private set; }

        public bool IsOrganizationOwner { get; private set; }
        
        public List<OrganizationMemberDto> Members { get; private set; }

        private readonly IOrganizationAppService _organizationAppService;
        private readonly IOrganizationMembershipAppService _organizationMembershipAppService;

        public ProfilePageModel(
            IOrganizationAppService organizationAppService,
            IOrganizationMembershipAppService organizationMembershipAppService)
        {
            _organizationAppService = organizationAppService;
            _organizationMembershipAppService = organizationMembershipAppService;
        }

        public async Task OnGetAsync()
        {
            await GetProfileAsync();
            await GetMembersAsync();

            IsOrganizationOwner = await _organizationAppService.IsOrganizationOwnerAsync(Organization.Id);
        }

        private async Task GetProfileAsync()
        {
            Organization = await _organizationAppService.GetProfileAsync(Name);
        }
        
        private async Task GetMembersAsync()
        {
            Members = (await _organizationMembershipAppService.GetMembersAsync(Organization.Id)).Items.ToList();
        }
    }
}
