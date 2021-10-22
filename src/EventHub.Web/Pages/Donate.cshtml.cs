using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Payment.PaymentRequests;
using Payment.Web.PaymentRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace EventHub.Web.Pages
{
    public class DonateModel : EventHubPageModel
    {
        private readonly IPaymentRequestAppService _paymentRequestAppService;
        private readonly IPaymentUrlBuilder _paymentUrlBuilder;

        public DonateModel(
            IPaymentRequestAppService paymentRequestAppService,
            IPaymentUrlBuilder paymentUrlBuilder)
        {
            _paymentRequestAppService = paymentRequestAppService;
            _paymentUrlBuilder = paymentUrlBuilder;
        }

        [BindProperty]
        public decimal Amount { get; set; } = 5;

        public async Task<IActionResult> OnPostAsync()
        {
            var paymentRequest = await _paymentRequestAppService.CreateAsync(new PaymentRequestCreationDto
            {
                CustomerId = CurrentUser.IsAuthenticated ? CurrentUser.Id.ToString() : Request.HttpContext.Connection.Id,
                Price = Amount,
                ProductId = "eventhub.donation",
                ProductName = $"EventHub Donation | " + (CurrentUser.UserName.IsNullOrWhiteSpace() ? "#" + Guid.NewGuid() : CurrentUser.UserName),
            });

            return Redirect(_paymentUrlBuilder.BuildCheckoutUrl(paymentRequest.Id).AbsoluteUri);
        }
    }
}
