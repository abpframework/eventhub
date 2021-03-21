using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace EventHub.Admin
{
    [DependsOn(
        typeof(EventHubDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(EventHubAdminApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule)
    )]
    public class EventHubAdminApplicationModule : AbpModule
    {
    }
}