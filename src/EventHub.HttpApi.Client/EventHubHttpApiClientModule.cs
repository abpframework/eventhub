using Microsoft.Extensions.DependencyInjection;
using Payment;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit.Public;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubApplicationContractsModule),
        typeof(AbpHttpClientModule),
        typeof(CmsKitPublicHttpApiClientModule),
        typeof(PaymentHttpApiClientModule)
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
            
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<EventHubHttpApiClientModule>();
            });
        }
    }
}
