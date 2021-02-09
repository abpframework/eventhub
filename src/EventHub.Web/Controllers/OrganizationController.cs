using System.IO;
using System.Threading.Tasks;
using EventHub.Organizations;
using EventHub.Web.Pages.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    [Route("api/organization")]
    public class OrganizationController : AbpController
    {
        private readonly IOrganizationAppService _organizationAppService;

        public OrganizationController(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }
        
        [HttpPost]
        [Authorize]
        [Route("save-profile-picture")]
        public async Task SaveProfilePicture([FromForm] OrganizationProfilePictureInput input)
        {
            byte[] profilePictureContent = new byte[] {};
            
            if (input.ProfilePictureFile != null && input.ProfilePictureFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await input.ProfilePictureFile.CopyToAsync(memoryStream);
                    profilePictureContent = memoryStream.ToArray();
                }
            }

            await _organizationAppService.SaveProfilePictureAsync(input.OrganizationId, profilePictureContent);
        }
    }
}