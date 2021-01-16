using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace EventHub.EntityFrameworkCore
{
    [DependsOn(
        typeof(EventHubEntityFrameworkCoreModule)
        )]
    public class EventHubEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<EventHubMigrationsDbContext>();
        }
    }
}
