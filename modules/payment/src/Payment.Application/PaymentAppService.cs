using Payment.Localization;
using Volo.Abp.Application.Services;

namespace Payment
{
    public abstract class PaymentAppService : ApplicationService
    {
        protected PaymentAppService()
        {
            LocalizationResource = typeof(PaymentResource);
            ObjectMapperContext = typeof(PaymentApplicationModule);
        }
    }
}
