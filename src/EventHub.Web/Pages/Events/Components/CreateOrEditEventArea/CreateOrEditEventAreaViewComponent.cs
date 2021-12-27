using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using EventHub.Web.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Users;

namespace EventHub.Web.Pages.Events.Components.CreateOrEditEventArea;

[Widget(
    AutoInitialize = true,
    RefreshUrl = "/Widgets/CreateOrEditEventArea",
    ScriptFiles = new[] { "/Pages/Events/Components/CreateOrEditEventArea/create-or-edit-event-area.js" },
    StyleFiles = new[] { "/Pages/Events/Components/CreateOrEditEventArea/create-or-edit-event-area.css" }
)]
public class CreateOrEditEventAreaViewComponent : AbpViewComponent
{
    private readonly IEventAppService _eventAppService;
    private readonly IOrganizationAppService _organizationAppService;

    private readonly ICurrentUser _currentUser;

    public CreateOrEditEventAreaViewComponent(
        IEventAppService eventAppService,
        IOrganizationAppService organizationAppService,
        ICurrentUser currentUser)
    {
        _eventAppService = eventAppService;
        _organizationAppService = organizationAppService;
        _currentUser = currentUser;
    }

    public async Task<IViewComponentResult> InvokeAsync(
        string eventUrlCode,
        ProgressStepType stepType = ProgressStepType.Event)
    {
        NewEventViewModel model = null;
        if (!eventUrlCode.IsNullOrWhiteSpace())
        {
            var @event = await _eventAppService.GetByUrlCodeAsync(eventUrlCode);
            model = ObjectMapper.Map<EventDetailDto, NewEventViewModel>(@event);
            ViewData["EventId"] = @event.Id;
            ViewData["EventUrlCode"] = @event.UrlCode;
        }

        model ??= new NewEventViewModel
        {
            StartTime = DateTime.Now.ClearTime().AddDays(1).AddHours(19),
            EndTime = DateTime.Now.ClearTime().AddDays(2).AddHours(21)
        };

        await FillViewDataAsync(stepType, model);

        return View("~/Pages/Events/Components/CreateOrEditEventArea/Default.cshtml", model);
    }

    private async Task<string> GetOrganizationNameAsync(Guid organizationId)
    {
        var organizationList = (await _organizationAppService.GetOrganizationsByUserIdAsync(_currentUser.GetId())).Items;

        return organizationList.Single(x => x.Id == organizationId).Name;
    }

    private async Task<List<SelectListItem>> GetOrganizationsSelectItemAsync()
    {
        var result = await _organizationAppService.GetOrganizationsByUserIdAsync(_currentUser.GetId());

        return result.Items.Select(
            organization => new SelectListItem
            {
                Value = organization.Id.ToString(),
                Text = organization.DisplayName
            }
        ).ToList();
    }

    private async Task<string> GetCountryNameAsync(Guid countryId)
    {
        var countries = await _eventAppService.GetCountriesLookupAsync();

        return countries.Single(x => x.Id == countryId).Name;
    }
    
    private async Task<List<SelectListItem>> GetCountriesSelectItemAsync()
    {
        var result = await _eventAppService.GetCountriesLookupAsync();

        return result.Select(
            country => new SelectListItem
            {
                Value = country.Id.ToString(),
                Text = country.Name
            }
        ).ToList();
    }
    
    private List<SelectListItem> GetLanguagesSelectItem()
    {
        var result = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
            .DistinctBy(x => x.EnglishName)
            .OrderBy(x => x.EnglishName)
            .ToList();
        result.Remove(result.Single(x => x.TwoLetterISOLanguageName == "iv")); // Invariant Language

        return result.Select(
            cultureInfo => new SelectListItem
            {
                Value = cultureInfo.TwoLetterISOLanguageName,
                Text = cultureInfo.EnglishName
            }
        ).ToList();
    }
    
    private async Task FillViewDataAsync(ProgressStepType stepType, NewEventViewModel model)
    {
        ViewData["StepType"] = stepType.ToString();
        switch (stepType)
        {
            case ProgressStepType.Event:
                ViewData["Organizations"] = await GetOrganizationsSelectItemAsync();
                ViewData["Countries"] = await GetCountriesSelectItemAsync();
                ViewData["Languages"] = GetLanguagesSelectItem();
                break;
            case ProgressStepType.Session:
                ViewData["Languages"] = GetLanguagesSelectItem();
                break;
            case ProgressStepType.Preview:
                ViewData["OrganizationName"] = await GetOrganizationNameAsync(model.OrganizationId);
                if (model.CountryId is not null)
                {
                    ViewData["CountryName"] = await GetCountryNameAsync(model.CountryId!.Value);
                }
                break;
        }
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
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
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

        [DisplayName("Language")] 
        public string Language { get; set; }

        [Range(1, int.MaxValue)] 
        public int? Capacity { get; set; }

        public List<TrackDto> Tracks { get; set; }

        public NewEventViewModel()
        {
            Tracks = new List<TrackDto>();
        }
    }

    public enum ProgressStepType : byte
    {
       Event = 0,
       Track,
       Session,
       Preview
    }
}
