using System.Threading.Tasks;

namespace Payment.PaymentRequests
{
    public class PaymentRequestAppService : PaymentAppService, IPaymentRequestAppService
    {
        private readonly IPaymentRequestRepository _paymentRequestRepository;

        public PaymentRequestAppService(IPaymentRequestRepository paymentRequestRepository)
        {
            _paymentRequestRepository = paymentRequestRepository;
        }
        
        public async Task<PaymentRequestDto> CreateAsync(PaymentRequestCreationDto input)
        {
            var paymentRequest = new PaymentRequest(
                GuidGenerator.Create(),
                input.CustomerId,
                input.ProductId,
                input.ProductName,
                input.Amount
            );

            await _paymentRequestRepository.InsertAsync(paymentRequest);

            return ObjectMapper.Map<PaymentRequest, PaymentRequestDto>(paymentRequest);
        }
    }
}