using Payment.Admin;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using Volo.CmsKit.Admin;

namespace EventHub.Admin
{
    [DependsOn(
        typeof(EventHubAdminApplicationContractsModule),
        typeof(AbpAccountHttpApiModule),
        typeof(AbpIdentityHttpApiModule),
        typeof(AbpPermissionManagementHttpApiModule),
        typeof(CmsKitAdminHttpApiModule),
        typeof(PaymentAdminHttpApiModule),
        typeof(AbpSettingManagementHttpApiModule)
    )]
    public class EventHubAdminHttpApiModule : AbpModule
    {
    }
}