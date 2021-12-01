using Microsoft.Extensions.DependencyInjection;
using Payment.EntityFrameworkCore.Repositories;
using Payment.PaymentRequests;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Payment.EntityFrameworkCore
{
    [DependsOn(
        typeof(PaymentDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
    )]
    public class PaymentEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<PaymentDbContext>(options =>
            {
                options.AddRepository<PaymentRequest, PaymentRequestRepository>();
            });
        }
    }
}