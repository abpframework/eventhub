using EventHub.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace EventHub.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(EventHubEntityFrameworkCoreDbMigrationsModule),
        typeof(EventHubApplicationContractsModule)
        )]
    public class EventHubDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
