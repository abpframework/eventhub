using Microsoft.Extensions.Options;
using Payment.PayPal;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.PaymentRequests
{
    public class PaymentRequestAppService : PaymentAppService, IPaymentRequestAppService
    {
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly PaymentOptions _paymentOptions;
        private readonly PayPalHttpClient _payPalHttpClient;

        public PaymentRequestAppService(
            IPaymentRequestRepository paymentRequestRepository,
            IOptions<PaymentOptions> paymentOptions,
            PayPalHttpClient payPalHttpClient)
        {
            _paymentRequestRepository = paymentRequestRepository;
            _paymentOptions = paymentOptions.Value;
            _payPalHttpClient = payPalHttpClient;
        }

        public async Task<PaymentRequestDto> GetAsync(Guid id)
        {
            var paymentRequest = await _paymentRequestRepository.GetAsync(id);
         
            return ObjectMapper.Map<PaymentRequest, PaymentRequestDto>(paymentRequest);
        }

        public async Task<PaymentRequestDto> CreateAsync(PaymentRequestCreationDto input)
        {
            var paymentRequest = new PaymentRequest(
                GuidGenerator.Create(),
                input.CustomerId,
                input.ProductId,
                input.ProductName,
                input.Price,
                input.Currency ?? _paymentOptions.DefaultCurrency
            );

            await _paymentRequestRepository.InsertAsync(paymentRequest);

            return ObjectMapper.Map<PaymentRequest, PaymentRequestDto>(paymentRequest);
        }

        public async Task<StartPaymentResultDto> StartPaymentAsync(StartPaymentDto input)
        {
            var paymentRequest = await _paymentRequestRepository.GetAsync(input.PaymentRequestId);

            var order = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    //ReturnUrl = _paymentUrlBuilder.BuildReturnUrl(PaymentRequest.Id).AbsoluteUri,
                    //CancelUrl = _paymentUrlBuilder.BuildCheckoutUrl(PaymentRequest.Id).AbsoluteUri,
                    ReturnUrl = input.ReturnUrl,
                    CancelUrl = input.CancelUrl,
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
                                    CurrencyCode = paymentRequest.Currency,
                                    Value = paymentRequest.Price.ToString(".00")
                                }
                            },
                            CurrencyCode = paymentRequest.Currency,
                            Value = paymentRequest.Price.ToString(".00"),
                        },
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Quantity = 1.ToString(),
                                Name = paymentRequest.ProductName,
                                UnitAmount = new Money
                                {
                                    CurrencyCode = paymentRequest.Currency,
                                    Value = paymentRequest.Price.ToString(".00")
                                }
                            }
                        },
                        ReferenceId = paymentRequest.Id.ToString()
                    }
                }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);

            var result = (await _payPalHttpClient.Execute(request)).Result<Order>();

            return new StartPaymentResultDto
            {
                CheckoutLink = result.Links.First(x => x.Rel == "approve").Href
            };
        }

        public async Task<PaymentRequestDto> CompleteAsync(string token)
        {
            var request = new OrdersCaptureRequest(token);
            request.RequestBody(new OrderActionRequest());

            var order = (await _payPalHttpClient.Execute(request)).Result<Order>();

            var paymentRequestId = Guid.Parse(order.PurchaseUnits.First().ReferenceId);

            var paymentRequest = await _paymentRequestRepository.GetAsync(paymentRequestId);

            if (order.Status == PayPalConsts.OrderStatus.Approved || order.Status == PayPalConsts.OrderStatus.Completed)
            {
                paymentRequest.SetAsCompleted();
            }
            else
            {
                paymentRequest.SetAsFailed(order.Status);
            }

            paymentRequest.ExtraProperties.Add(PayPalConsts.OrderIdPropertyName, order.Id);

            await _paymentRequestRepository.UpdateAsync(paymentRequest);

            return ObjectMapper.Map<PaymentRequest, PaymentRequestDto>(paymentRequest);
        }
    }
}