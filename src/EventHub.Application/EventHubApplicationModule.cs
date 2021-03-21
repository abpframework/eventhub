using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(EventHubApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule)
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
