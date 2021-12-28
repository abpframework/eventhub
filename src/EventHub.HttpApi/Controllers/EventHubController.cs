using EventHub.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Controllers
{
    public abstract class EventHubController : AbpControllerBase
    {
        protected EventHubController()
        {
            LocalizationResource = typeof(EventHubResource);
        }
    }
}
