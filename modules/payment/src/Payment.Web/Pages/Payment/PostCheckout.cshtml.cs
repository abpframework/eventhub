using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payment.PaymentRequests;
using Payment.Web.PaymentRequest;
using System;
using System.Threading.Tasks;

namespace Payment.Web.Pages.Payment
{
    public class PostCheckoutPageModel : PaymentPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        public PaymentRequestDto PaymentRequest { get; protected set; }

        public string GoBackLink { get; set; }

        protected IPaymentRequestAppService PaymentRequestAppService { get; }
        protected PaymentWebOptions PaymentWebOptions { get; }
        protected IPaymentUrlBuilder PaymentUrlBuilder { get; }

        public PostCheckoutPageModel(
            IPaymentRequestAppService appService,
            IOptions<PaymentWebOptions> paymentWebOptions,
            IPaymentUrlBuilder paymentUrlBuilder)
        {
            PaymentRequestAppService = appService;
            PaymentWebOptions = paymentWebOptions.Value;
            PaymentUrlBuilder = paymentUrlBuilder;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            if (Token.IsNullOrWhiteSpace())
            {
                return BadRequest();
            }

            PaymentRequest = await PaymentRequestAppService.CompleteAsync(Token);

            GoBackLink = PaymentUrlBuilder.BuildCheckoutUrl(PaymentRequest.Id).AbsoluteUri;

            if (PaymentRequest.State == PaymentRequestState.Completed 
                && !PaymentWebOptions.PaymentSuccessfulCallbackUrl.IsNullOrWhiteSpace())
            {
                var callbackUrl = PaymentWebOptions.PaymentSuccessfulCallbackUrl + "?paymentRequestId=" + PaymentRequest.Id;
                Response.Redirect(callbackUrl);
            }

            if (PaymentRequest.State == PaymentRequestState.Failed
                && !PaymentWebOptions.PaymentFailureCallbackUrl.IsNullOrWhiteSpace())
            {
                var callbackUrl = PaymentWebOptions.PaymentFailureCallbackUrl + "?paymentRequestId=" + PaymentRequest.Id;
                Response.Redirect(callbackUrl);
            }

            return Page();
        }
    }
}
