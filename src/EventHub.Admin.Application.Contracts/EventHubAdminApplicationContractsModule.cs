using Payment.Admin;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace EventHub.Admin
{
    [DependsOn(
        typeof(EventHubDomainSharedModule),
        typeof(AbpAccountApplicationContractsModule),
        typeof(AbpIdentityApplicationContractsModule),
        typeof(AbpPermissionManagementApplicationContractsModule),
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