using Microsoft.Extensions.Options;
using Payment.PayPal;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Payment.PaymentRequests
{
    public class PaymentRequestAppService : PaymentAppService, IPaymentRequestAppService
    {
        protected readonly IPaymentRequestRepository _paymentRequestRepository;
        protected readonly PaymentOptions _paymentOptions;
        protected readonly PayPalHttpClient _payPalHttpClient;

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
            
            foreach (var extraProperty in input.ExtraProperties)
            {
                paymentRequest.SetProperty(extraProperty.Key, extraProperty.Value);
            }

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

            var paymentRequest = await UpdatePaymentRequestStateAsync(order);

            return ObjectMapper.Map<PaymentRequest, PaymentRequestDto>(paymentRequest);
        }

        public async Task<bool> HandleWebhookAsync(string payload)
        {
            var jObject = JObject.Parse(payload);

            //// TODO: Find way to parse with System.Text.Json instead of Newtonsoft
            var order = jObject["resource"].ToObject<Order>();

            var request = new OrdersGetRequest(order.Id);

            // Ensure order object comes from PayPal
            var response = await _payPalHttpClient.Execute(request);
            order = response.Result<Order>();

            await UpdatePaymentRequestStateAsync(order);

            // PayPal doesn't accept Http 204 (NoContent) result and tries to execute webhook again.
            // So with following value, API returns Http 200 (OK) result.
            return true;
        }

        private async Task<PaymentRequest> UpdatePaymentRequestStateAsync(Order order)
        {
            var paymentRequestId = Guid.Parse(order.PurchaseUnits.First().ReferenceId);

            var paymentRequest = await _paymentRequestRepository.GetAsync(paymentRequestId);

            if (order.Status is PayPalConsts.OrderStatus.Completed or PayPalConsts.OrderStatus.Approved)
            {
                paymentRequest.SetAsCompleted();
            }
            else
            {
                paymentRequest.SetAsFailed(order.Status);
            }

            paymentRequest.ExtraProperties[PayPalConsts.OrderIdPropertyName] = order.Id;
            paymentRequest.ExtraProperties[nameof(order.Status)] = order.Status;

            await _paymentRequestRepository.UpdateAsync(paymentRequest);

            return paymentRequest;
        }
    }
}