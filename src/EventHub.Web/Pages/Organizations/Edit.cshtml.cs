using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EventHub.Organizations;
using EventHub.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Pages.Organizations
{
    [Authorize]
    public class EditPageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }
        
        [BindProperty] 
        public EditOrganizationViewModel Organization { get; set; }
        
        private readonly IOrganizationAppService _organizationAppService;

        public EditPageModel(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            await GetOrganizationAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ValidateModel();

                var input = ObjectMapper.Map<EditOrganizationViewModel, UpdateOrganizationDto>(Organization);
                await _organizationAppService.UpdateAsync(Organization.Id, input);
                
                return RedirectToPage("./Profile", new { name = Name });
            }
            catch (Exception exception)
            {
                ShowAlert(exception);
                return Page();
            }
        }

        private async Task GetOrganizationAsync()
        {
            var organizationProfileDto = await _organizationAppService.GetProfileAsync(Name);
            Organization = ObjectMapper.Map<OrganizationProfileDto, EditOrganizationViewModel>(organizationProfileDto);
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
        [StringLength(OrganizationConsts.MaxDescriptionNameLength, MinimumLength = OrganizationConsts.MinDescriptionNameLength)]
        public string Description { get; set; }
        
        public string Website { get; set; }

        public string TwitterUsername { get; set; }

        public string GitHubUsername { get; set; }

        public string FacebookUsername { get; set; }

        public string InstagramUsername { get; set; }

        public string MediumUsername { get; set; }
    }

    public class OrganizationProfilePictureInput
    {
        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [MaxFileSize(OrganizationConsts.MaxProfilePictureFileSize)] 
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile ProfilePictureFile { get; set; }
    }
}