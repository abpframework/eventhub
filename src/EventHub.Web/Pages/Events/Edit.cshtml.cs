using EventHub.Events;

namespace EventHub.Web.Pages.Events
{
    public class EditPageModel : EventHubPageModel
    {
        public string UrlCode { get; set; }
        
        public EditPageModel()
        {
            
        }
        
        public void OnGet(string url)
        {
            UrlCode = EventUrlCodeHelper.GetCodeFromUrl(url);
        }
    }
}
