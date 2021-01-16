using Volo.Abp.Bundling;

namespace EventHub.Blazor
{
    public class EventHubBundleContributor : IBundleContributor
    {
        public void AddScripts(BundleContext context)
        {
        }

        public void AddStyles(BundleContext context)
        {
            context.Add("main.css", true);
        }
    }
}