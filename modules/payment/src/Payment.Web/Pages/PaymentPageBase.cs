using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Localization;
using Payment.Localization;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.Layout;

namespace Payment.Web.Pages
{
    public class PaymentPageBase : Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        [RazorInject] public IStringLocalizer<PaymentResource> L { get; set; }

        [RazorInject] public IPageLayout PageLayout { get; set; }

        public override Task ExecuteAsync()
        {
            return Task.CompletedTask; // Will be overriden by razor pages. (.cshtml)
        }
    }
}
