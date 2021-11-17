using System;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace EventHub.Admin.Web.Menus
{
    public class EventHubMenuContributor : IMenuContributor
    {
        private readonly IConfiguration _configuration;

        public EventHubMenuContributor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
            else if (context.Menu.Name == StandardMenus.User)
            {
                await ConfigureUserMenuAsync(context);
            }
        }

        private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<EventHubResource>();

            var eventHubMenuItem = new ApplicationMenuItem(
                EventHubMenus.Prefix,
                "EventHub"
            );

            context.Menu.Items.Insert(0, eventHubMenuItem);
            
            eventHubMenuItem.AddItem(
                new ApplicationMenuItem(
                    EventHubMenus.OrganizationManagement.Organizations,
                    l["Menu:Organizations"],
                    url: "/organizations"
                ).RequirePermissions(EventHubPermissions.Organizations.Default)
            );

            eventHubMenuItem.AddItem(
                new ApplicationMenuItem(
                    EventHubMenus.OrganizationManagement.OrganizationMemberships,
                    l["Menu:OrganizationMemberships"],
                    url: "/organization-memberships"
                ).RequirePermissions(EventHubPermissions.Organizations.Memberships.Default)
            );
            
            eventHubMenuItem
                .AddItem(
                    new ApplicationMenuItem(
                        EventHubMenus.EventManagement.Events,
                        displayName: l["Menu:Events"],
                        url: "/events"
                    ).RequirePermissions(EventHubPermissions.Events.Default)
                );
            
            return Task.CompletedTask;
        }

        private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
        {
            var accountStringLocalizer = context.GetLocalizer<AccountResource>();
            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

            var identityServerUrl = _configuration["AuthServer:Authority"] ?? "";

            if (currentUser.IsAuthenticated)
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    "Account.Manage",
                    accountStringLocalizer["Manage"],
                    $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={_configuration["App:SelfUrl"]}",
                    icon: "fa fa-cog",
                    order: 1000,
                    null));
            }

            return Task.CompletedTask;
        }
    }
}