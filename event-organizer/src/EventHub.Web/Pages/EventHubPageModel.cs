using EventHub.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EventHub.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class EventHubPageModel : AbpPageModel
    {
        protected EventHubPageModel()
        {
            LocalizationResourceType = typeof(EventHubResource);
        }
    }
}