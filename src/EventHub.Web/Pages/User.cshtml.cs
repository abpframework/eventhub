using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Members;
using EventHub.Organizations;
using EventHub.Users;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Pages
{
    public class UserModel : EventHubPageModel
    {
        private readonly IUserAppService _userAppService;
        private readonly IOrganizationAppService _organizationAppService;

        public List<OrganizationInListDto> Organizations { get; set; }

        public UserModel(
            IUserAppService userAppService, 
            IOrganizationAppService organizationAppService)
        {
            _userAppService = userAppService;
            _organizationAppService = organizationAppService;
        }

        public new UserDto User { get; set; }

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
            
            User = await _userAppService.FindByUserNameAsync(userName);
            Organizations = (await _organizationAppService.GetOrganizationsByUserIdAsync(User.Id)).Items.ToList();

            if (User != null)
            {
                return Page();
            }

            return Redirect("/events");
        }
    }
}