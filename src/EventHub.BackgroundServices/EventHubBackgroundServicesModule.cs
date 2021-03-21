using EventHub.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace EventHub
{

    [DependsOn(
        typeof(AbpAutofacModule)
    )]
    public class EventHubBackgroundServicesModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();

            context.Services.AddHostedService<EventHubBackgroundServicesHostedService>();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorker<EventReminderWorker>();
            context.AddBackgroundWorker<NewEventDetectorWorker>();
            context.AddBackgroundWorker<EventTimingChangeWorker>();
        }
    }
}
