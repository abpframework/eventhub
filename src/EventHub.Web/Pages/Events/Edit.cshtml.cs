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
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.Content;
using Volo.Abp.Users;

namespace EventHub.Web.Pages.Events
{
    public class EditPageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Url { get; set; }
        
        [BindProperty] 
        public EditEventViewModel Event { get; set; }
        
        public List<SelectListItem> Organizations { get; private set; }
        public List<SelectListItem> Countries { get; private set; }
        public List<SelectListItem> Languages { get; private set; }

        private readonly IEventAppService _eventAppService;
        private readonly IOrganizationAppService _organizationAppService;

        public EditPageModel(
            IEventAppService eventAppService,
            IOrganizationAppService organizationAppService)
        {
            _eventAppService = eventAppService;
            _organizationAppService = organizationAppService;
        }
        
        public async Task OnGetAsync()
        {
            var urlCode = EventUrlCodeHelper.GetCodeFromUrl(Url);
            var eventDetailDto = await _eventAppService.GetByUrlCodeAsync(urlCode);

            Event = ObjectMapper.Map<EventDetailDto, EditEventViewModel>(eventDetailDto);
            
            await FillOrganizationsAsync();
            await FillCountriesAsync();
            FillLanguages();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ValidateModel();

                var updateEventDto = ObjectMapper.Map<EditEventViewModel, UpdateEventDto>(Event);
                
                await using var memoryStream = new MemoryStream();
                if (Event.CoverImageFile != null && Event.CoverImageFile.Length > 0)
                {
                    await Event.CoverImageFile.CopyToAsync(memoryStream);
                    updateEventDto.CoverImageStreamContent = new RemoteStreamContent(memoryStream)
                    {
                        ContentType = Event.CoverImageFile.ContentType,
                        FileName = Event.CoverImageFile.FileName,
                    };
                }
                
                await _eventAppService.UpdateAsync(Event.Id, updateEventDto);
                await memoryStream.DisposeAsync();

                return RedirectToPage("./Detail", new { url = Url });
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
            var result = await _organizationAppService.GetOrganizationsByUserIdAsync(CurrentUser.GetId());
            Organizations = result.Items.Select(
                organization => new SelectListItem
                {
                    Value = organization.Id.ToString(),
                    Text = organization.DisplayName
                }
            ).ToList();

            if (Event.OrganizationId != Guid.Empty)
            {
                Organizations.Single(x => x.Value == Event.OrganizationId.ToString()).Selected = true;
            }
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

            if (Event.CountryId.HasValue)
            {
                Countries.Single(x => x.Value == Event.CountryId.ToString()).Selected = true;
            }
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

            if (!string.IsNullOrWhiteSpace(Event.Language))
            {
                Languages.Single(x => x.Value == Event.Language).Selected = true;
            }
        }
        
        public class EditEventViewModel
        {
            [HiddenInput]
            public Guid Id { get; set; }
            
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
            [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
            public IFormFile CoverImageFile { get; set; }

            [Required]
            public bool IsOnline { get; set; }
            
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
            
            [Range(1, int.MaxValue)]
            public int? Capacity { get; set; }
        }
    }
}