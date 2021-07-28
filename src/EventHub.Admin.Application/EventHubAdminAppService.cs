using EventHub.Localization;
using Volo.Abp.Application.Services;

namespace EventHub.Admin
{
    public class EventHubAdminAppService : ApplicationService
    {
        protected EventHubAdminAppService()
        {
            LocalizationResource = typeof(EventHubResource);
        }
    }
}
