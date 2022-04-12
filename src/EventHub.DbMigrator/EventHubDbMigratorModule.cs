using EventHub.Admin;
using EventHub.EntityFrameworkCore;
using EventHub.Organizations;
using EventHub.Organizations.Plans;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace EventHub.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(EventHubEntityFrameworkCoreModule),
        typeof(EventHubApplicationContractsModule),
        typeof(EventHubAdminApplicationContractsModule)
        )]
    public class EventHubDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
            
#if DEBUG
            Configure<PlanInfoOptions>(options =>
            {
                options.Infos.Add(new PlanInfoDefinition
                {
                    PlanType = OrganizationPlanType.Free,
                    Description = "This is a default plan.",
                    IsActive = true,
                    Price = 0,
                    IsExtendable = false
                });
            });
#endif
        }
    }
}
