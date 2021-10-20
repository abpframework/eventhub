using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payment.PaymentRequests;
using System;
using System.Threading.Tasks;

namespace Payment.Web.Pages.Payment
{
    public class PostCheckoutPageModel : PaymentPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        public PaymentRequestDto PaymentRequest { get; protected set; }

        private readonly IPaymentRequestAppService _paymentRequestAppService;
        private readonly PaymentWebOptions _paymentWebOptions;

        public PostCheckoutPageModel(
            IPaymentRequestAppService appService,
            IOptions<PaymentWebOptions> paymentWebOptions)
        {
            _paymentRequestAppService = appService;
            _paymentWebOptions = paymentWebOptions.Value;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Token.IsNullOrWhiteSpace())
            {
                return BadRequest();
            }

            PaymentRequest = await _paymentRequestAppService.CompleteAsync(Token);

            if (PaymentRequest.State == PaymentRequestState.Completed 
                && !_paymentWebOptions.PaymentSuccessfulCallbackUrl.IsNullOrWhiteSpace())
            {
                var callbackUrl = _paymentWebOptions.PaymentSuccessfulCallbackUrl + "?paymentRequestId=" + PaymentRequest.Id;
                Response.Redirect(callbackUrl);
            }

            if (PaymentRequest.State == PaymentRequestState.Failed
                && !_paymentWebOptions.PaymentFailureCallbackUrl.IsNullOrWhiteSpace())
            {
                var callbackUrl = _paymentWebOptions.PaymentFailureCallbackUrl + "?paymentRequestId=" + PaymentRequest.Id;
                Response.Redirect(callbackUrl);
            }

            return Page();
        }
    }
}
