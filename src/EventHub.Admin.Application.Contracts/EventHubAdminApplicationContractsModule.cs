using Payment.Admin;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.CmsKit.Admin;

namespace EventHub.Admin
{
    [DependsOn(
        typeof(EventHubDomainSharedModule),
        typeof(AbpAccountApplicationContractsModule),
        typeof(AbpIdentityApplicationContractsModule),
        typeof(AbpPermissionManagementApplicationContractsModule),
        typeof(CmsKitAdminApplicationContractsModule),
        typeof(PaymentAdminApplicationContractsModule)
    )]
    public class EventHubAdminApplicationContractsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            EventHubDtoExtensions.Configure();
        }
    }
}