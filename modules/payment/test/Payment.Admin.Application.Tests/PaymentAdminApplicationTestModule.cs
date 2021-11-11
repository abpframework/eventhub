using Volo.Abp.Modularity;

namespace Payment.Admin.Application.Tests
{
    [DependsOn(
        typeof(PaymentDomainTestModule),
        typeof(PaymentAdminApplicationModule)
    )]
    public class PaymentAdminApplicationTestModule : AbpModule
    {
        
    }
}