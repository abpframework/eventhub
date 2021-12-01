using Payment.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Payment.Admin
{
    public abstract class PaymentAdminComponentBase : AbpComponentBase
    {
        protected PaymentAdminComponentBase()
        {
            LocalizationResource = typeof(PaymentResource);
            ObjectMapperContext = typeof(PaymentAdminBlazorModule);
        }
    }
}