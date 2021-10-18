using System;
using System.Threading.Tasks;
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
                    ProductName = "My product 1",
                    Price = 99.99m
                }
            );
            
            result.Id.ShouldNotBe(Guid.Empty);
            result.Price.ShouldBe(99.99m);
            result.ProductName.ShouldBe("My product 1");
        }
    }
}