using System.Threading.Tasks;
using Payment.Admin.Permissions;
using Payment.Localization;
using Volo.Abp.UI.Navigation;

namespace Payment.Admin.Navigation
{
    public class PaymentAdminBlazorMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            await AddPaymentMenuAsync(context);
        }

        private Task AddPaymentMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<PaymentResource>();

            var paymentMenu = context.Menu.FindMenuItem(PaymentAdminMenuNames.GroupName);
            if (paymentMenu == null)
            {
                paymentMenu = new ApplicationMenuItem(
                    PaymentAdminMenuNames.GroupName,
                    l["Menu:PaymentManagement"],
                    icon: "fa fa-money-check",
                    url: "/payment/requests",
                    requiredPermissionName: PaymentAdminPermissions.Request.Default
                );
            }

            context.Menu.AddItem(paymentMenu);
            return Task.CompletedTask;
        }
    }
}