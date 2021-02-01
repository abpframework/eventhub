using EventHub.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubEntityFrameworkCoreTestModule)
        )]
    public class EventHubDomainTestModule : AbpModule
    {

    }
}