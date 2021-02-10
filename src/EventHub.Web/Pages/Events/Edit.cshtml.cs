using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUglify.Helpers;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace EventHub.Web.Pages.Events
{
    public class EditPageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Url { get; set; }
        
        [BindProperty] 
        public EditEventViewModel Event { get; set; }
        
        public List<SelectListItem> Countries { get; private set; }
        public List<SelectListItem> Languages { get; private set; }

        private readonly IEventAppService _eventAppService;

        public EditPageModel(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }
        
        public async Task OnGetAsync()
        {
            var urlCode = EventUrlCodeHelper.GetCodeFromUrl(Url);

            var eventDetailDto = await _eventAppService.GetByUrlCodeAsync(urlCode);
            Event = ObjectMapper.Map<EventDetailDto, EditEventViewModel>(eventDetailDto);
            
            FillLanguages();
            await FillCountriesAsync();
            await GetLocationInfoAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ValidateModel();

                var input = ObjectMapper.Map<EditEventViewModel, UpdateEventDto>(Event);
                await _eventAppService.UpdateAsync(Event.Id, input);
                
                return RedirectToPage("./Detail", new { url = Url });
            }
            catch (Exception exception)
            {
                ShowAlert(exception);
                FillLanguages();
                await FillCountriesAsync();
                return Page();
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

        private async Task GetLocationInfoAsync()
        {
            var eventLocationDto = await _eventAppService.GetLocationAsync(Event.Id);
            Event.City = eventLocationDto.City;
            Event.OnlineLink = eventLocationDto.OnlineLink;

            Event.CountryId = Guid.TryParse(
                Countries.FirstOrDefault(x => x.Text == eventLocationDto.Country)?.Value,
                out var countryId
            )
                ? countryId
                : null;
        }

        public class EditEventViewModel
        {
            [HiddenInput]
            public Guid Id { get; set; }
            
            [Required]
            [StringLength(EventConsts.MaxTitleLength, MinimumLength = EventConsts.MinTitleLength)]
            public string Title { get; set; }

            [Required]
            [StringLength(EventConsts.MaxDescriptionLength, MinimumLength = EventConsts.MinDescriptionLength)]
            [TextArea]
            public string Description { get; set; }

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