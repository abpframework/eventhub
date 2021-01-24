using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventHub.Web.Pages.Events
{
    public class Detail : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Url { get; set; }

        public EventDetailDto Event { get; set; }

        private readonly IEventAppService _eventAppService;

        public Detail(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public async Task OnGetAsync()
        {
            var urlCode = EventUrlCodeHelper.GetCodeFromUrl(Url);

            Event = await _eventAppService.GetByUrlCodeAsync(urlCode);
        }
    }
}
