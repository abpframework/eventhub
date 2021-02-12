using EventHub.Events;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.VirtualFileSystem;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(EventHubApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpBackgroundWorkersModule)
        )]
    public class EventHubApplicationModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorker<EventReminderWorker>();
            context.AddBackgroundWorker<NewEventDetectorWorker>();
            context.AddBackgroundWorker<EventTimingChangeWorker>();
        }
        
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<EventHubApplicationModule>();
            });
            
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<EventHubApplicationModule>("EventHub");
            });
        }
    }
}
