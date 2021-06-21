using System;
using System.Threading.Tasks;
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

        public bool IsShowSocialMediaContent { get; set; }
        
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

            if (!Organization.Website.IsNullOrWhiteSpace() || !Organization.TwitterUsername.IsNullOrWhiteSpace() || !Organization.GitHubUsername.IsNullOrWhiteSpace() || !Organization.FacebookUsername.IsNullOrWhiteSpace() || !Organization.InstagramUsername.IsNullOrWhiteSpace() || !Organization.MediumUsername.IsNullOrWhiteSpace())
            {
                IsShowSocialMediaContent = true;
            }
        }
    }
}
