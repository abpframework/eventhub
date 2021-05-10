using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using EventHub.Web.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUglify.Helpers;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace EventHub.Web.Pages.Events
{
    public class NewPageModel : EventHubPageModel
    {
        [BindProperty] public NewEventViewModel Event { get; set; }

        public List<SelectListItem> Organizations { get; private set; }
        public List<SelectListItem> Countries { get; private set; }
        public List<SelectListItem> Languages { get; private set; }

        private readonly IEventAppService _eventAppService;
        private readonly IOrganizationAppService _organizationAppService;

        public NewPageModel(
            IEventAppService eventAppService,
            IOrganizationAppService organizationAppService)
        {
            _eventAppService = eventAppService;
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            Event = new NewEventViewModel
            {
                StartTime = DateTime.Now.ClearTime().AddDays(1).AddHours(19),
                EndTime = DateTime.Now.ClearTime().AddDays(1).AddHours(21)
            };

            await FillOrganizationsAsync();
            await FillCountriesAsync();
            FillLanguages();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ValidateModel();

                var input = ObjectMapper.Map<NewEventViewModel, CreateEventDto>(Event);

                if (Event.CoverImageFile != null && Event.CoverImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Event.CoverImageFile.CopyToAsync(memoryStream);
                        input.CoverImageContent = memoryStream.ToArray();
                    }
                }

                var eventDto = await _eventAppService.CreateAsync(input);

                return RedirectToPage("/Events/Detail", new {url = eventDto.UrlCode});
            }
            catch (Exception exception)
            {
                ShowAlert(exception);
                await FillOrganizationsAsync();
                await FillCountriesAsync();
                FillLanguages();
                return Page();
            }
        }

        private async Task FillOrganizationsAsync()
        {
            var result = await _organizationAppService.GetMyOrganizationsAsync();
            Organizations = result.Items.Select(
                organization => new SelectListItem
                {
                    Value = organization.Id.ToString(),
                    Text = organization.DisplayName
                }
            ).ToList();
        }

        private async Task FillCountriesAsync()
        {
            var result = await _eventAppService.GetCountriesLookupAsync();

            Countries = result.Select(
                country => new SelectListItem
                {
                    Value = country.Id.ToString(),
                    Text = country.Name
                }
            ).ToList();
        }

        private void FillLanguages()
        {
            var result = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                .DistinctBy(x => x.EnglishName)
                .OrderBy(x => x.EnglishName)
                .ToList();
            result.Remove(result.Single(x => x.TwoLetterISOLanguageName == "iv")); // Invariant Language

            Languages = result.Select(
                cultureInfo => new SelectListItem
                {
                    Value = cultureInfo.TwoLetterISOLanguageName,
                    Text = cultureInfo.EnglishName
                }
            ).ToList();
        }

        public class NewEventViewModel
        {
            [SelectItems(nameof(Organizations))]
            [DisplayName("Organization")]
            public Guid OrganizationId { get; set; }

            [Required]
            [StringLength(EventConsts.MaxTitleLength, MinimumLength = EventConsts.MinTitleLength)]
            public string Title { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime StartTime { get; set; } = DateTime.Now;

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime EndTime { get; set; } = DateTime.Now;

            [Required]
            [StringLength(EventConsts.MaxDescriptionLength, MinimumLength = EventConsts.MinDescriptionLength)]
            [TextArea]
            public string Description { get; set; }

            [CanBeNull]
            [Display(Name = "Cover Image")]
            [DataType(DataType.Upload)]
            [MaxFileSize(EventConsts.MaxCoverImageFileSize)]
            [AllowedExtensions(new string[] {".jpg", ".png", ".jpeg"})]
            public IFormFile CoverImageFile { get; set; }

            [Required] public bool? IsOnline { get; set; }

            [CanBeNull]
            [StringLength(EventConsts.MaxOnlineLinkLength, MinimumLength = EventConsts.MinOnlineLinkLength)]
            public string OnlineLink { get; set; }

            [SelectItems(nameof(Countries))]
            [DisplayName("Country")]
            public Guid? CountryId { get; set; }

            [CanBeNull]
            [StringLength(EventConsts.MaxCityLength, MinimumLength = EventConsts.MinCityLength)]
            public string City { get; set; }

            [SelectItems(nameof(Languages))]
            [DisplayName("Language")]
            public string Language { get; set; }

            [Range(1, int.MaxValue)] public int? Capacity { get; set; }
        }
    }
}