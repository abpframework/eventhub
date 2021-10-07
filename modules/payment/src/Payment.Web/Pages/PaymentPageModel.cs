using Payment.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Payment.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class PaymentPageModel : AbpPageModel
    {
        protected PaymentPageModel()
        {
            LocalizationResourceType = typeof(PaymentResource);
            ObjectMapperContext = typeof(PaymentWebModule);
        }
    }
}