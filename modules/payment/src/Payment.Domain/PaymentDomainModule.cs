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

    }
}
