using Microsoft.AspNetCore.Mvc;
using Payment.PaymentRequests;
using Payment.Web.PaymentRequest;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Payment.Web.Pages.Payment
{
    public class PreCheckoutPageModel : PaymentPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid PaymentRequestId { get; set; }

        public PaymentRequestDto PaymentRequest { get; set; }

        protected IPaymentRequestAppService PaymentRequestAppService { get; }
        protected IPaymentUrlBuilder PaymentUrlBuilder { get; }

        public PreCheckoutPageModel(
            IPaymentRequestAppService paymentRequestAppService,
            IPaymentUrlBuilder paymentUrlBuilder)
        {
            PaymentRequestAppService = paymentRequestAppService;
            PaymentUrlBuilder = paymentUrlBuilder;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            PaymentRequest = await PaymentRequestAppService.GetAsync(PaymentRequestId);

            return Page();
        }

        public virtual async Task OnPostAsync()
        {
            PaymentRequest = await PaymentRequestAppService.GetAsync(PaymentRequestId);

            var result = await PaymentRequestAppService.StartPaymentAsync(new StartPaymentDto
            {
                PaymentRequestId = PaymentRequest.Id,
                ReturnUrl = PaymentUrlBuilder.BuildReturnUrl(PaymentRequestId).AbsoluteUri,
                CancelUrl = PaymentUrlBuilder.BuildCheckoutUrl(PaymentRequestId).AbsoluteUri
            });

            Response.Redirect(result.CheckoutLink);
        }
    }
}
