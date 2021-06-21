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
    public class NewPageModel : EventHubPageModel
    {
        [BindProperty] public CreateOrganizationViewModel Organization { get; set; }

        private readonly IOrganizationAppService _organizationAppService;

        public NewPageModel(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public void OnGet()
        {
            Organization = new CreateOrganizationViewModel();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ValidateModel();

                var input = ObjectMapper.Map<CreateOrganizationViewModel, CreateOrganizationDto>(Organization);

                if (Organization.ProfilePictureFile != null && Organization.ProfilePictureFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Organization.ProfilePictureFile.CopyToAsync(memoryStream);
                        input.ProfilePictureContent = memoryStream.ToArray();
                    }
                }

                await _organizationAppService.CreateAsync(input);

                return RedirectToPage("./Profile", new {name = Organization.Name});
            }
            catch (Exception exception)
            {
                ShowAlert(exception);
                return Page();
            }
        }

        public class CreateOrganizationViewModel
        {
            [Required]
            [StringLength(OrganizationConsts.MaxNameLength, MinimumLength = OrganizationConsts.MinNameLength)]
            public string Name { get; set; }

            [Required]
            [StringLength(OrganizationConsts.MaxDisplayNameLength, MinimumLength = OrganizationConsts.MinDisplayNameLength)]
            public string DisplayName { get; set; }

            [Required]
            [StringLength(OrganizationConsts.MaxDescriptionNameLength, MinimumLength = OrganizationConsts.MinDescriptionNameLength)]
            [TextArea]
            public string Description { get; set; }

            [CanBeNull]
            [DataType(DataType.Upload)]
            [MaxFileSize(OrganizationConsts.MaxProfilePictureFileSize)]
            [AllowedExtensions(new string[] {".jpg", ".png", ".jpeg"})]
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
}