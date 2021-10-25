using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Authorization;

namespace EventHub.Web.Pages
{
    public class Pricing : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string OrganizationName { get; set; }
        
        public OrganizationProfileDto Organization { get; private set; }

        private readonly IOrganizationAppService _organizationAppService;

        public Pricing(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            Organization = await _organizationAppService.GetProfileAsync(OrganizationName);

            if (CurrentUser.UserName != Organization.OwnerUserName)
            {
                throw new AbpAuthorizationException();
            }
        }

        public async Task OnPostAsync()
        {
            
        }
    }
}