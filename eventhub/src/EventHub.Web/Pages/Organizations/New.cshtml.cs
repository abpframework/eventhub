using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace EventHub.Web.Pages.Organizations
{
    [Authorize]
    public class NewPageModel : EventHubPageModel
    {
        [BindProperty]
        public CreateOrganizationViewModel Organization { get; set; }

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
        }
    }
}
