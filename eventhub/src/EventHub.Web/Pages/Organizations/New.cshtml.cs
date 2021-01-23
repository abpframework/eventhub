using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace EventHub.Web.Pages.Organizations
{
    [Authorize]
    public class New : EventHubPageModel
    {
        [BindProperty]
        public CreateOrganizationViewModel Organization { get; set; }

        private readonly IOrganizationAppService _organizationAppService;
        private readonly IExceptionToErrorInfoConverter _errorInfoConverter;

        public New(
            IOrganizationAppService organizationAppService,
            IExceptionToErrorInfoConverter errorInfoConverter)
        {
            _organizationAppService = organizationAppService;
            _errorInfoConverter = errorInfoConverter;
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

                return RedirectToPage("./OrganizationProfile", new {name = Organization.Name});
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                var errorInfo = _errorInfoConverter.Convert(exception, false);
                Alerts.Danger(errorInfo.Message);
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
