using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;

namespace EventHub.Web.Theme
{
    [ThemeName(Name)]
    public class EventHubTheme : ITheme, ITransientDependency
    {
        public const string Name = "EventHub";

        public virtual string GetLayout(string name, bool fallbackToDefault = true)
        {
            switch (name)
            {
                case StandardLayouts.Application:
                    return "~/Themes/EventHub/Layouts/Application.cshtml";
                case StandardLayouts.Account:
                    return "~/Themes/EventHub/Layouts/Account.cshtml";
                case StandardLayouts.Empty:
                    return "~/Themes/EventHub/Layouts/Empty.cshtml";
                default:
                    return fallbackToDefault ? "~/Themes/EventHub/Layouts/Application.cshtml" : null;
            }
        }
    }
}
