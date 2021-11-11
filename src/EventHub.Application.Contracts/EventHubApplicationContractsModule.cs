using Payment;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(PaymentApplicationContractsModule)
    )]
    public class EventHubApplicationContractsModule : AbpModule
    {

    }
}
