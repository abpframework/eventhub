using Payment.Admin.Navigation;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace Payment.Admin
{
    [DependsOn(
        typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule),
        typeof(PaymentAdminHttpApiClientModule)
        )]
    public class PaymentAdminBlazorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new PaymentAdminBlazorMenuContributor());
            });
            
            Configure<AbpRouterOptions>(options =>
            {
                options.AdditionalAssemblies.Add(typeof(PaymentAdminBlazorModule).Assembly);
            });
        }
    }
}