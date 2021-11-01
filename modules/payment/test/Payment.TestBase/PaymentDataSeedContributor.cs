using System.Threading.Tasks;
using Payment.PaymentRequests;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Payment
{
    public class PaymentDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly PaymentTestData _paymentTestData;

        public PaymentDataSeedContributor(IPaymentRequestRepository paymentRequestRepository, PaymentTestData paymentTestData)
        {
            _paymentRequestRepository = paymentRequestRepository;
            _paymentTestData = paymentTestData;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await BuildPaymentRequestAsync();
        }

        private async Task BuildPaymentRequestAsync()
        {
            var paymentRequest = new PaymentRequest(
                _paymentTestData.PaymentRequest1Id,
                _paymentTestData.Customer1Id,
                _paymentTestData.Product1Id,
                "Product 1",
                10,
                "USD");

            await _paymentRequestRepository.InsertAsync(paymentRequest, autoSave: true);
        }
    }
}
