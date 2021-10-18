using Microsoft.AspNetCore.Mvc;
using Payment.PaymentRequests;
using Payment.Web.PaymentRequest;
using System;
using System.Threading.Tasks;

namespace Payment.Web.Pages.Payment
{
    public class CheckoutPageModel : PaymentPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid PaymentRequestId { get; set; }
        
        public PaymentRequestDto PaymentRequest { get; set; }

        private readonly IPaymentRequestAppService _paymentRequestAppService;
        private readonly IPaymentUrlBuilder _paymentUrlBuilder;

        public CheckoutPageModel(
            IPaymentRequestAppService paymentRequestAppService,
            IPaymentUrlBuilder paymentUrlBuilder)
        {
            _paymentRequestAppService = paymentRequestAppService;
            this._paymentUrlBuilder = paymentUrlBuilder;
        }
        
        public async Task OnGetAsync()
        {
            PaymentRequest = await _paymentRequestAppService.GetAsync(PaymentRequestId);
        }
        
        public async Task OnPostAsync()
        {
            var result = await _paymentRequestAppService.StartPaymentAsync(new StartPaymentDto
            {
                PaymentRequestId = PaymentRequestId,
                ReturnUrl = _paymentUrlBuilder.BuildReturnUrl(PaymentRequestId).AbsoluteUri,
                CancelUrl = _paymentUrlBuilder.BuildCheckoutUrl(PaymentRequestId).AbsoluteUri
            });

            Response.Redirect(result.CheckoutLink);
        }
    }
}