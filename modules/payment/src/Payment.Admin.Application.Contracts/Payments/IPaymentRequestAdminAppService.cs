using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Payment.Admin.Payments
{
    public interface IPaymentRequestAdminAppService : IApplicationService
    {
        Task<PagedResultDto<PaymentRequestWithDetailsDto>> GetListAsync(PaymentRequestGetListInput input);
    }
}