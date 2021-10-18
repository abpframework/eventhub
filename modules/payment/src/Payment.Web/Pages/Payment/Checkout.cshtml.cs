using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUglify.Helpers;
using Payment.PaymentRequests;
using Payment.Web.PaymentRequest;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace Payment.Web.Pages.Payment
{
    public class CheckoutPageModel : PaymentPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid PaymentRequestId { get; set; }
        
        public PaymentRequestDto PaymentRequest { get; set; }

        private readonly IPaymentRequestAppService _paymentRequestAppService;
        private readonly IPaymentUrlBuilder _paymentUrlBuilder;
        private readonly PayPalHttpClient _payPalHttpClient;

        public CheckoutPageModel(
            IPaymentRequestAppService paymentRequestAppService,
            IPaymentUrlBuilder paymentUrlBuilder,
            PayPalHttpClient payPalHttpClient)
        {
            _paymentRequestAppService = paymentRequestAppService;
            this._paymentUrlBuilder = paymentUrlBuilder;
            this._payPalHttpClient = payPalHttpClient;
        }
        
        public async Task OnGetAsync()
        {
            PaymentRequest = await _paymentRequestAppService.GetAsync(PaymentRequestId);
        }
        
        public async Task OnPostAsync()
        {
            PaymentRequest = await _paymentRequestAppService.GetAsync(PaymentRequestId);

            var order = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = _paymentUrlBuilder.BuildReturnUrl(PaymentRequest.Id).AbsoluteUri,
                    CancelUrl = _paymentUrlBuilder.BuildCheckoutUrl(PaymentRequest.Id).AbsoluteUri,
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = PaymentRequest.Currency,
                                    Value = PaymentRequest.Price.ToString(".00")
                                }
                            },
                            CurrencyCode = PaymentRequest.Currency,
                            Value = PaymentRequest.Price.ToString(".00"),
                        },
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Quantity = 1.ToString(),
                                Name = PaymentRequest.ProductName,
                                UnitAmount = new Money
                                {
                                    CurrencyCode = PaymentRequest.Currency,
                                    Value = PaymentRequest.Price.ToString(".00")
                                }
                            }
                        },
                        ReferenceId = PaymentRequest.Id.ToString()
                    }
                }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);

            var result = (await _payPalHttpClient.Execute(request)).Result<Order>();

            Response.Redirect(result.Links.First(x => x.Rel == "approve").Href);
        }
    }
}