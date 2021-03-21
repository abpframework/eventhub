using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubDomainModule),
        typeof(EventHubApplicationContractsModule),
        typeof(AbpDddApplicationModule)
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
