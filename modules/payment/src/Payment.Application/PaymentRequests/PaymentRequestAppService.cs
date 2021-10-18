using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Payment.PaymentRequests
{
    public class PaymentRequestAppService : PaymentAppService, IPaymentRequestAppService
    {
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly PaymentOptions _paymentOptions;

        public PaymentRequestAppService(
            IPaymentRequestRepository paymentRequestRepository,
            IOptions<PaymentOptions> paymentOptions)
        {
            _paymentRequestRepository = paymentRequestRepository;
            _paymentOptions = paymentOptions.Value;
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
    }
}