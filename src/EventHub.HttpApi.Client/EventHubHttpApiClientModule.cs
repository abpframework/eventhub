using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubApplicationContractsModule),
        typeof(AbpHttpClientModule)
    )]
    public class EventHubHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(EventHubApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
