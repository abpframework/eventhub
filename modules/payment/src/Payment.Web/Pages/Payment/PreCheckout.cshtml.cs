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

        private readonly IPaymentRequestAppService _paymentRequestAppService;
        private readonly IPaymentUrlBuilder _paymentUrlBuilder;

        public PreCheckoutPageModel(
            IPaymentRequestAppService paymentRequestAppService,
            IPaymentUrlBuilder paymentUrlBuilder)
        {
            _paymentRequestAppService = paymentRequestAppService;
            _paymentUrlBuilder = paymentUrlBuilder;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            PaymentRequest = await _paymentRequestAppService.GetAsync(PaymentRequestId);

            return Page();
        }
        
        public async Task OnPostAsync()
        {
            PaymentRequest = await _paymentRequestAppService.GetAsync(PaymentRequestId);

            if (PaymentRequest.CreationTime.AddMinutes(15) >= Clock.Now)
            {
                throw new UserFriendlyException("Payment requiest is timed out.");
            }

            var result = await _paymentRequestAppService.StartPaymentAsync(new StartPaymentDto
            {
                PaymentRequestId = PaymentRequest.Id,
                ReturnUrl = _paymentUrlBuilder.BuildReturnUrl(PaymentRequestId).AbsoluteUri,
                CancelUrl = _paymentUrlBuilder.BuildCheckoutUrl(PaymentRequestId).AbsoluteUri
            });

            Response.Redirect(result.CheckoutLink);
        }
    }
}