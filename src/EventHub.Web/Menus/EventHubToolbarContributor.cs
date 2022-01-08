using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Web.Components.Toolbar.CreateButton;
using EventHub.Web.Components.Toolbar.LoginLink;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.Users;

namespace EventHub.Web.Menus
{
    public class EventHubToolbarContributor : IToolbarContributor
    {
        public virtual Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
        {
            if (context.Toolbar.Name != StandardToolbars.Main)
            {
                return Task.CompletedTask;
            }

            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

            if (currentUser.IsAuthenticated)
            {
                context.Toolbar.Items.AddFirst(new ToolbarItem(typeof(CreateButtonViewComponent)));
            }

            if (!currentUser.IsAuthenticated)
            {
                context.Toolbar.Items.Add(new ToolbarItem(typeof(LoginLinkViewComponent)));
            }

            return Task.CompletedTask;
        }
    }
}
