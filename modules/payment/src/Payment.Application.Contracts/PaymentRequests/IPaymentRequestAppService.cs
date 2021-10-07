using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Payment.PaymentRequests
{
    public interface IPaymentRequestAppService : IApplicationService
    {
        Task<PaymentRequestDto> CreateAsync(PaymentRequestCreationDto input);
    }
}