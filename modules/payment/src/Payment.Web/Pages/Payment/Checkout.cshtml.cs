using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payment.PaymentRequests;

namespace Payment.Web.Pages.Payment
{
    public class CheckoutPageModel : PaymentPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid PaymentRequestId { get; set; }
        
        public PaymentRequestDto PaymentRequest { get; set; }

        private readonly IPaymentRequestAppService _paymentRequestAppService;

        public CheckoutPageModel(IPaymentRequestAppService paymentRequestAppService)
        {
            _paymentRequestAppService = paymentRequestAppService;
        }
        
        public async Task OnGetAsync()
        {
            PaymentRequest = await _paymentRequestAppService.GetAsync(PaymentRequestId);
        }
        
        public async Task OnPostAsync()
        {
            PaymentRequest = await _paymentRequestAppService.GetAsync(PaymentRequestId);

            // TODO: Redirect from here.



        }
    }
}