using Microsoft.Extensions.DependencyInjection;
using Payment.PayPal;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Payment
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(PaymentDomainSharedModule)
    )]
    public class PaymentDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<PayPalOptions>(configuration.GetSection("Payment:PayPal"));
            Configure<PaymentOptions>(configuration.GetSection("Payment"));
        }
    }
}
