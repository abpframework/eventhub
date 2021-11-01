using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Payment.PaymentRequests
{
    public class PaymentRequestAppService_Tests : PaymentApplicationTestBase
    {
        private readonly IPaymentRequestAppService _paymentRequestAppService;

        public PaymentRequestAppService_Tests()
        {
            _paymentRequestAppService = GetRequiredService<IPaymentRequestAppService>();
        }

        [Fact]
        public async Task Should_Create_Minimal_Payment_Request()
        {
            var result = await _paymentRequestAppService.CreateAsync(
                new PaymentRequestCreationDto
                {
                    CustomerId = "customer_1",
                    ProductId = "my_product_1",
                    ProductName = "My product 1",
                    Price = 99.99m
                }
            );

            result.Id.ShouldNotBe(Guid.Empty);
            result.Price.ShouldBe(99.99m);
            result.ProductName.ShouldBe("My product 1");
        }

        [Fact]
        public async Task Should_Create_Payment_Request_With_Default_Currency()
        {
            var result = await _paymentRequestAppService.CreateAsync(new PaymentRequestCreationDto
            {
                CustomerId = "customer_1",
                ProductId = "my_product_1",
                Price = 4.95m,
                ProductName = "Donation"
            });

            var options = GetRequiredService<IOptions<PaymentOptions>>();

            result.Currency.ShouldBe(options.Value.DefaultCurrency);
        }
    }
}
