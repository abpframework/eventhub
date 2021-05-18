using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Members;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Pages.Members
{
    public class ProfileModel : EventHubPageModel
    {
        private readonly IMemberAppService _memberAppService;
        private readonly IOrganizationAppService _organizationAppService;

        public List<OrganizationInListDto> Organizations { get; set; }

        public ProfileModel(
            IMemberAppService memberAppService, 
            IOrganizationAppService organizationAppService)
        {
            _memberAppService = memberAppService;
            _organizationAppService = organizationAppService;
        }

        public UserDto User { get; set; }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            if (userName.IsNullOrWhiteSpace())
            {
                if (CurrentUser.IsAuthenticated)
                {
                    userName = CurrentUser.UserName;                    
                }
                else
                {
                    return Redirect("/events");
                }
            }
            
            User = await _memberAppService.FindByUserNameAsync(userName);
            Organizations = (await _organizationAppService.GetOrganizationsByUserIdAsync(User.Id)).Items.ToList();

            if (User != null)
            {
                return Page();
            }

            return Redirect("/events");
        }
    }
}