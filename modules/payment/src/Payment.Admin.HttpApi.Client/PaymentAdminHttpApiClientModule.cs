using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Payment.Admin
{
    [DependsOn(
        typeof(PaymentAdminApplicationContractsModule),
        typeof(AbpHttpClientModule)
    )]
    public class PaymentAdminHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddStaticHttpClientProxies(
                typeof(PaymentAdminApplicationContractsModule).Assembly,
                PaymentAdminRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<PaymentAdminHttpApiClientModule>();
            });
        }
    }
}
