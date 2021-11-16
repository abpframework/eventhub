using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EventHub.Web.Pages.Events
{
    public class DetailPageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public new string Url { get; set; }

        public bool IsEventOwner { get; set; }

        public EventDetailDto Event { get; set; }

        private readonly IEventAppService _eventAppService;
        private readonly LinkGenerator _linkGenerator;

        public DetailPageModel(
            IEventAppService eventAppService,
            LinkGenerator linkGenerator)
        {
            _eventAppService = eventAppService;
            _linkGenerator = linkGenerator;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var urlCode = EventUrlCodeHelper.GetCodeFromUrl(Url);

            Event = await _eventAppService.GetByUrlCodeAsync(urlCode);

            if (Event.Url != Url)
            {
                return RedirectPermanent(
                    _linkGenerator.GetPathByPage(
                        "/Events/Detail",
                        values: new
                        {
                            url = Event.Url
                        }
                    )!
                );
            }

            IsEventOwner = await _eventAppService.IsEventOwnerAsync(Event.Id);

            return Page();
        }
    }
}
