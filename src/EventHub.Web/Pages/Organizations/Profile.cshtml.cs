using System;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace EventHub.Web.Pages.Organizations
{
    public class ProfilePageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        public OrganizationProfileDto Organization { get; private set; }

        public bool IsOrganizationOwner { get; private set; }

        public bool IsShowSocialMediaContent { get; set; }

        public bool HasPastEvents { get; set; } 
        
        public bool HasUpcomingEvents { get; set; } 
        
        private readonly IOrganizationAppService _organizationAppService;
        private readonly IEventAppService _eventAppService;

        public ProfilePageModel(
            IOrganizationAppService organizationAppService,
            IOrganizationMembershipAppService organizationMembershipAppService,
            IEventAppService eventAppService)
        {
            _organizationAppService = organizationAppService;
            _eventAppService = eventAppService;
        }

        public async Task OnGetAsync()
        {
            await GetProfileAsync();

            IsOrganizationOwner = await _organizationAppService.IsOrganizationOwnerAsync(Organization.Id);

            HasPastEvents = (await _eventAppService.GetListAsync(new EventListFilterDto()
            {
                OrganizationId = Organization.Id,
                MaxDate = Clock.Now.ClearTime()
            })).TotalCount > 0;
            
            HasUpcomingEvents = (await _eventAppService.GetListAsync(new EventListFilterDto()
            {
                OrganizationId = Organization.Id,
                MinDate = Clock.Now.ClearTime()
            })).TotalCount > 0;
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
