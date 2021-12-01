using Volo.Abp.Modularity;

namespace Payment
{
    [DependsOn(
        typeof(PaymentDomainModule)
        )]
    public class PaymentBackgroundServicesModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            
        }
    }
}