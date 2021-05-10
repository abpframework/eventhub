using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Organizations;
using EventHub.Web.Pages.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
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
        
        [HttpGet]
        [Route("get-list")]
        public async Task<IActionResult> GetList(OrganizationListFilterDto input)
        {
            ViewData.Model = (await _organizationAppService.GetListAsync(input)).Items.ToList();
            
            return new PartialViewResult
            {
                ViewName = "~/Pages/Organizations/Components/OrganizationsArea/_organizationListSection.cshtml",
                ViewData = ViewData
            };
        }
        
        [HttpPost]
        [Authorize]
        [Route("save-profile-picture")]
        public async Task SaveProfilePicture([FromForm] OrganizationProfilePictureInput input)
        {
            var profilePictureContent = new byte[] {};
            
            if (input.ProfilePictureFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await input.ProfilePictureFile.CopyToAsync(memoryStream);
                    profilePictureContent = memoryStream.ToArray();
                }

                await _organizationAppService.SaveProfilePictureAsync(input.OrganizationId, profilePictureContent);
            }
        }
    }
}