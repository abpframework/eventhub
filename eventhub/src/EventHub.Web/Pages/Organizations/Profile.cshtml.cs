using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventHub.Web.Pages.Organizations
{
    public class ProfilePageModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        public OrganizationProfileDto Organization { get; set; }

        private readonly IOrganizationAppService _organizationAppService;

        public ProfilePageModel(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            Organization = await _organizationAppService.GetProfileAsync(Name);
        }
    }
}
