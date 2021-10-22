using System;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Payment.Admin
{
    [DependsOn(
        typeof(PaymentDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class PaymentAdminApplicationContractsModule : AbpModule
    {

    }
}
