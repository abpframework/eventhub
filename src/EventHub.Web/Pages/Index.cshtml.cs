using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Authentication;

namespace EventHub.Web.Pages
{
    public class IndexModel : EventHubPageModel
    {
        public IReadOnlyList<EventInListDto> Events { get; private set; }

        private readonly IEventAppService _eventAppService;

        public IndexModel(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public async Task OnGetAsync()
        {
            var result = await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MinDate = Clock.Now
                }
            );

            Events = result.Items;
        }

        public async Task OnPostLoginAsync()
        {
            await HttpContext.ChallengeAsync("oidc");
        }
    }
}
