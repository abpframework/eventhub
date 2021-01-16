using EventHub.Localization;
using Volo.Abp.Application.Services;

namespace EventHub
{
    /* Inherit your application services from this class.
     */
    public abstract class EventHubAppService : ApplicationService
    {
        protected EventHubAppService()
        {
            LocalizationResource = typeof(EventHubResource);
        }
    }
}
