using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Organizations;

namespace EventHub.Web.Pages
{
    public class ProfileModel : EventHubPageModel
    {
        private readonly IOrganizationAppService _organizationAppService;

        public IReadOnlyList<OrganizationInListDto> MyOrganizations { get; set; }
        
        public ProfileModel(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            MyOrganizations = (await _organizationAppService.GetMyOrganizationsAsync()).Items;
        }
    }
}