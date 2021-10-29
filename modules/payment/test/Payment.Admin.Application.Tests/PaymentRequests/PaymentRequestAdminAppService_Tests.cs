using System.Threading.Tasks;
using Payment.Admin.Payments;
using Shouldly;
using Xunit;

namespace Payment.Admin.Application.Tests.PaymentRequests
{
    public class PaymentRequestAdminAppService_Tests : PaymentAdminApplicationTestBase
    {
        private readonly IPaymentRequestAdminAppService _paymentRequestAdminAppService;

        public PaymentRequestAdminAppService_Tests()
        {
            _paymentRequestAdminAppService = GetRequiredService<IPaymentRequestAdminAppService>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            var paymentRequests = await _paymentRequestAdminAppService.GetListAsync(new PaymentRequestGetListInput());
            paymentRequests.Items.ShouldNotBeNull();
        }
    }
}