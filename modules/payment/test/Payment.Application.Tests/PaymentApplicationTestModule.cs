using Volo.Abp.Modularity;

namespace Payment
{
    [DependsOn(
        typeof(PaymentApplicationModule),
        typeof(PaymentDomainTestModule)
        )]
    public class PaymentApplicationTestModule : AbpModule
    {

    }
}
