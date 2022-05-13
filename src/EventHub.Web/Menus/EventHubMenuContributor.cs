using System;
using System.Threading.Tasks;
using EventHub.Localization;
using EventHub.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace EventHub.Web.Menus
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

            context.Menu.Items.Insert(0, new ApplicationMenuItem(EventHubMenus.Home, l["Menu:Home"], "~/"));
            context.Menu.Items.Insert(1, new ApplicationMenuItem(EventHubMenus.Organizations, l["Menu:Organizations"], "~/organizations"));

            return Task.CompletedTask;
        }

        private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<EventHubResource>();
            var accountStringLocalizer = context.GetLocalizer<AccountResource>();
            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

            var identityServerUrl = _configuration["AuthServer:Authority"] ?? "";

            if (currentUser.IsAuthenticated)
            {
                context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountStringLocalizer["ManageYourProfile"],
                    $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={EventHubUrlOptions.GetWwwConfigValue(_configuration)}", icon: "fa fa-cog", order: 1000, null, "_blank"));
                context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", l["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000));
            }

            return Task.CompletedTask;
        }
    }
}
