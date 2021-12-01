using Payment.Admin;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;

namespace EventHub.Admin
{
    [DependsOn(
        typeof(EventHubAdminApplicationContractsModule),
        typeof(AbpAccountHttpApiModule),
        typeof(AbpIdentityHttpApiModule),
        typeof(AbpPermissionManagementHttpApiModule),
        typeof(PaymentAdminHttpApiModule)
    )]
    public class EventHubAdminHttpApiModule : AbpModule
    {
    }
}