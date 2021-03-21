using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubDomainSharedModule),
        typeof(AbpDddApplicationContractsModule)
    )]
    public class EventHubApplicationContractsModule : AbpModule
    {

    }
}
