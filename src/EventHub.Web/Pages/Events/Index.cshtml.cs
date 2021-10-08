using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Pages.Events
{
    public class IndexModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid? OrganizationId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? MinDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? MaxDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? IsOnline { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string Language { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public Guid? CountryId { get; set; }
        
        public List<CultureInfo> Languages { get; private set; }
        public List<CountryLookupDto> Countries { get; private set; }
        
        private readonly IEventAppService _eventAppService;

        public IndexModel(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }
        
        public async Task OnGetAsync()
        {
            await FillCountriesAsync();
            FillLanguages();
        }

        private async Task FillCountriesAsync()
        {
            Countries = await _eventAppService.GetCountriesLookupAsync();
            Countries.Insert(0, new CountryLookupDto
            {
                Id = Guid.Empty,
                Name = "Online"
            });
        }

        private void FillLanguages()
        {
            Languages = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                .DistinctBy(x => x.EnglishName)
                .OrderBy(x => x.EnglishName)
                .ToList();
            Languages.Remove(Languages.Single(x => x.TwoLetterISOLanguageName == "iv")); // Invariant Language
        }
    }
}