using Payment.Admin;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.CmsKit.Admin;

namespace EventHub.Admin
{
    [DependsOn(
        typeof(EventHubDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(EventHubAdminApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(CmsKitAdminApplicationModule),
        typeof(PaymentAdminApplicationModule),
        typeof(AbpSettingManagementApplicationModule)
    )]
    public class EventHubAdminApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<EventHubAdminApplicationAutoMapperProfile>(validate: true);
            });
        }
    }
}