using Microsoft.Extensions.DependencyInjection;
using Payment.Admin;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.CmsKit.Admin;

namespace EventHub.Admin
{
    [DependsOn(
        typeof(EventHubAdminApplicationContractsModule),
        typeof(AbpAccountHttpApiClientModule),
        typeof(AbpIdentityHttpApiClientModule),
        typeof(AbpPermissionManagementHttpApiClientModule),
        typeof(CmsKitAdminHttpApiClientModule),
        typeof(PaymentAdminHttpApiClientModule),
        typeof(AbpSettingManagementHttpApiClientModule)
        )]
    public class EventHubAdminHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(EventHubAdminApplicationContractsModule).Assembly,
                EventHubAdminRemoteServiceConsts.RemoteServiceName
            );
        }
    }
}