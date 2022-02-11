using Payment;
using Volo.Abp.Application;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.CmsKit.Public;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(CmsKitPublicApplicationContractsModule),
        typeof(PaymentApplicationContractsModule)
    )]
    public class EventHubApplicationContractsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            LimitedResultRequestDto.MaxMaxResultCount = 30;
        }
    }
}
