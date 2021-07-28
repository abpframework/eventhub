using System;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
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

        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<EventHubResource>();

            context.Menu.Items.Insert(
                0,
                new ApplicationMenuItem(
                    EventHubMenus.Home,
                    l["Menu:Home"],
                    "/",
                    icon: "fas fa-home"
                )
            );
            
            await AddOrganizationMenu(context, l);
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
                    accountStringLocalizer["ManageYourProfile"],
                    $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={_configuration["App:SelfUrl"]}",
                    icon: "fa fa-cog",
                    order: 1000,
                    null));
            }

            return Task.CompletedTask;
        }
        
        private Task AddOrganizationMenu(MenuConfigurationContext context, IStringLocalizer l)
        {
            var organizationMenu = new ApplicationMenuItem(EventHubMenus.OrganizationManagement.GroupName, l["Menu:OrganizationManagement"], icon: "fa fa-sitemap");
            context.Menu.Items.Insert(2, organizationMenu);

            organizationMenu.AddItem(new ApplicationMenuItem(EventHubMenus.OrganizationManagement.Organizations, l["Organizations"], url: "/organizations").RequirePermissions(EventHubPermissions.Organizations.Default));
            organizationMenu.AddItem(new ApplicationMenuItem(EventHubMenus.OrganizationManagement.OrganizationMemberships, l["OrganizationMemberships"], url: "/organization-memberships").RequirePermissions(EventHubPermissions.Organizations.Memberships.Default));
            
            return Task.CompletedTask;
        }
    }
}
