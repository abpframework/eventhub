using EventHub.EntityFrameworkCore;
using EventHub.Events;
using EventHub.Organizations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace EventHub
{

    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpBackgroundWorkersModule),
        typeof(EventHubEntityFrameworkCoreModule),
        typeof(AbpCachingStackExchangeRedisModule)
    )]
    public class EventHubBackgroundServicesModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();

            context.Services.AddHostedService<EventHubBackgroundServicesHostedService>();
            
            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "EventHub:";
            });
            
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            context.Services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "EventHub-Protection-Keys");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorkerAsync<EventReminderWorker>();
            context.AddBackgroundWorkerAsync<NewEventWorker>();
            context.AddBackgroundWorkerAsync<EventTimingChangeWorker>();
            
            context.AddBackgroundWorkerAsync<OrganizationPaidEnrollmentEndDateWorker>();
        }
    }
}
