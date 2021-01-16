using EventHub.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EventHub.Web.Pages
{
    public abstract class EventHubPageModel : AbpPageModel
    {
        protected EventHubPageModel()
        {
            LocalizationResourceType = typeof(EventHubResource);
        }
    }
}