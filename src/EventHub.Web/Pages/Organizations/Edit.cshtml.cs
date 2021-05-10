using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using EventHub.Organizations;
using EventHub.Web.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace EventHub.Web.Pages.Organizations
{
    [Authorize]
    public class EditPageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }
        
        [BindProperty] 
        public EditOrganizationViewModel Organization { get; set; }
        public byte[] ProfilePictureContent { get; private set; }

        private readonly IOrganizationAppService _organizationAppService;

        public EditPageModel(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            var organizationProfileDto = await _organizationAppService.GetProfileAsync(Name);
            ProfilePictureContent = organizationProfileDto.ProfilePictureContent;
            
            Organization = ObjectMapper.Map<OrganizationProfileDto, EditOrganizationViewModel>(organizationProfileDto);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ValidateModel();

                var input = ObjectMapper.Map<EditOrganizationViewModel, UpdateOrganizationDto>(Organization);
                
                if (Organization.ProfilePictureFile != null && Organization.ProfilePictureFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Organization.ProfilePictureFile.CopyToAsync(memoryStream);
                        input.ProfilePictureContent = memoryStream.ToArray();
                    }
                }
                
                await _organizationAppService.UpdateAsync(Organization.Id, input);
                
                return RedirectToPage("./Profile", new { name = Name });
            }
            catch (Exception exception)
            {
                ShowAlert(exception);
                return Page();
            }
        }
    }

    public class EditOrganizationViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(OrganizationConsts.MaxDisplayNameLength, MinimumLength = OrganizationConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(OrganizationConsts.MaxDescriptionNameLength, MinimumLength = OrganizationConsts.MinDescriptionNameLength)]
        public string Description { get; set; }
        
        [CanBeNull]
        [DataType(DataType.Upload)]
        [MaxFileSize(OrganizationConsts.MaxProfilePictureFileSize)] 
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile ProfilePictureFile { get; set; }
        
        [StringLength(OrganizationConsts.MaxWebsiteLength)]
        public string Website { get; set; }
        
        [StringLength(OrganizationConsts.MaxTwitterUsernameLength)]
        public string TwitterUsername { get; set; }
        
        [StringLength(OrganizationConsts.MaxGitHubUsernameLength)]
        public string GitHubUsername { get; set; }
        
        [StringLength(OrganizationConsts.MaxFacebookUsernameLength)]
        public string FacebookUsername { get; set; }
        
        [StringLength(OrganizationConsts.MaxInstagramUsernameLength)]
        public string InstagramUsername { get; set; }
        
        [StringLength(OrganizationConsts.MaxMediumUsernameLength)]
        public string MediumUsername { get; set; }
    }
}