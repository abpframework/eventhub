using Volo.Abp.Modularity;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubApplicationModule),
        typeof(EventHubDomainTestModule)
        )]
    public class EventHubApplicationTestModule : AbpModule
    {

    }
}