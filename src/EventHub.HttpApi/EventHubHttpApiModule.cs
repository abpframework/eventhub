using Localization.Resources.AbpUi;
using EventHub.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Payment;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(PaymentHttpApiModule)
        )]
    public class EventHubHttpApiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            ConfigureLocalization();
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<EventHubResource>()
                    .AddBaseTypes(
                        typeof(AbpUiResource)
                    );
            });
        }
    }
}
