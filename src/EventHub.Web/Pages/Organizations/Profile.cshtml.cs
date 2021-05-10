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
        
        private readonly IOrganizationAppService _organizationAppService;

        public ProfilePageModel(
            IOrganizationAppService organizationAppService,
            IOrganizationMembershipAppService organizationMembershipAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            await GetProfileAsync();

            IsOrganizationOwner = await _organizationAppService.IsOrganizationOwnerAsync(Organization.Id);
        }

        private async Task GetProfileAsync()
        {
            Organization = await _organizationAppService.GetProfileAsync(Name);
        }
    }
}
