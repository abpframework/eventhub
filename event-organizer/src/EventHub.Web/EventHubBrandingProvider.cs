using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace EventHub.Web
{
    [Dependency(ReplaceServices = true)]
    public class EventHubBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "EventHub";
    }
}
