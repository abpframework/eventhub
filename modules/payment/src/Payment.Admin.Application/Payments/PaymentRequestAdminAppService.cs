using System.Collections.Generic;
using System.Threading.Tasks;
using Payment.PaymentRequests;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Payment.Admin.Payments
{
    public class PaymentRequestAdminAppService : ApplicationService, IPaymentRequestAdminAppService
    {
        protected IPaymentRequestRepository PaymentRequestRepository { get; }

        public PaymentRequestAdminAppService(IPaymentRequestRepository paymentRequestRepository)
        {
            PaymentRequestRepository = paymentRequestRepository;
        }
        
        public async Task<PagedResultDto<PaymentRequestWithDetailsDto>> GetListAsync(PaymentRequestGetListInput input)
        {
            var paymentRequests = await PaymentRequestRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter,
                input.CreationDateMax,
                input.CreationDateMin,
                input.Status
            );

            var totalCount = await PaymentRequestRepository.GetCountAsync(
                input.Filter,
                input.CreationDateMax,
                input.CreationDateMin,
                input.Status
            );

            return new PagedResultDto<PaymentRequestWithDetailsDto>(
                totalCount,
                ObjectMapper.Map<List<PaymentRequest>, List<PaymentRequestWithDetailsDto>>(paymentRequests)
            );
        }
    }
}