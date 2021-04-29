using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUglify.Helpers;

namespace EventHub.Web.Pages
{
    public class IndexModel : EventHubPageModel
    {
        public IReadOnlyList<EventInListDto> Events { get; private set; }
        
        public IReadOnlyList<EventInListDto> OnlineEvents { get; private set; }

        public List<CountryLookupDto> Countries { get; private set; }

        private readonly IEventAppService _eventAppService;

        public IndexModel(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public async Task OnGetAsync()
        {
            Events = (await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MinDate = Clock.Now
                }
            )).Items;

            OnlineEvents = (await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MinDate = Clock.Now,
                    IsOnline = true,
                    MaxResultCount = 8
                }
            )).Items;
            
            Countries = await _eventAppService.GetCountriesLookupAsync();
        }

        public async Task OnPostLoginAsync()
        {
            await HttpContext.ChallengeAsync("oidc");
        }
    }
}
