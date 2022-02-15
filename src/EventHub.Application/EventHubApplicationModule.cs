using Payment;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.CmsKit.Public;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubDomainModule),
        typeof(EventHubApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(CmsKitPublicApplicationModule),
        typeof(PaymentApplicationModule)
    )]
    public class EventHubApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<EventHubApplicationModule>();
            });
        }
    }
}
