using Payment.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Payment.Admin
{
    public abstract class PaymentAdminController : AbpControllerBase
    {
        protected PaymentAdminController()
        {
            LocalizationResource = typeof(PaymentResource);
        }
    }
}