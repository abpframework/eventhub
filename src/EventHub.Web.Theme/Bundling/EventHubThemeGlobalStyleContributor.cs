using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace EventHub.Web.Theme.Bundling
{
    public class EventHubThemeGlobalStyleContributor : BundleContributor
    {
        public override void ConfigureBundle(BundleConfigurationContext context)
        {
            context.Files.Add("/themes/eventhub/style.css");
            context.Files.Add("/themes/eventhub/owl-edit.css");
            context.Files.Add("/themes/eventhub/floating-labels.css");
        }
    }
}
