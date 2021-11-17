using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Authentication;

namespace EventHub.Web.Pages
{
    public class IndexModel : EventHubPageModel
    {
        public IReadOnlyList<EventInListDto> OnlineEvents { get; private set; }
        
        public List<CultureInfo> Languages { get; private set; }

        public List<CountryLookupDto> Countries { get; private set; }

        private readonly IEventAppService _eventAppService;

        public IndexModel(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public async Task OnGetAsync()
        {
            await FillOnlineEventsAsync();
            await FillCountriesAsync();
            FillLanguages();
        }

        public async Task OnPostLoginAsync()
        {
            await HttpContext.ChallengeAsync("oidc");
        }
        
        private async Task FillOnlineEventsAsync()
        {
            OnlineEvents = (await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MinDate = Clock.Now.ClearTime(),
                    IsOnline = true,
                    MaxResultCount = 8
                }
            )).Items;
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
