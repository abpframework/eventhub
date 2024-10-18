using EventHub.EntityFrameworkCore;
using EventHub.Web.Shared.HealthChecks;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;

namespace EventHub.Web.Shared;

[DependsOn(typeof(AbpAspNetCoreModule), typeof(EventHubEntityFrameworkCoreModule))]
public class EventHubWebSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddEventHubHealthChecks();
    }
}
