using Payment.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Payment
{
    public abstract class PaymentController : AbpControllerBase
    {
        protected PaymentController()
        {
            LocalizationResource = typeof(PaymentResource);
        }
    }
}
