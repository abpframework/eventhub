using Localization.Resources.AbpUi;
using Payment.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Payment
{
    [DependsOn(
        typeof(PaymentApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class PaymentHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(PaymentHttpApiModule).Assembly);
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
