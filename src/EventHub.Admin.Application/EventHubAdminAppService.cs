using EventHub.Localization;
using Volo.Abp.Application.Services;

namespace EventHub.Admin.Application
{
    public class EventHubAdminAppService : ApplicationService
    {
        public EventHubAdminAppService()
        {
            LocalizationResource = typeof(EventHubResource);
        }
    }
}
