using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Payment.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Payment.Admin
{
    [DependsOn(
        typeof(PaymentAdminApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule)
        )]
    public class PaymentAdminHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(PaymentAdminHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<PaymentResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
