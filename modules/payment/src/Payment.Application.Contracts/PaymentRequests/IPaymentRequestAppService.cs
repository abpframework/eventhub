using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Payment.PaymentRequests
{
    public interface IPaymentRequestAppService : IApplicationService
    {
        Task<PaymentRequestDto> GetAsync(Guid id);
        
        Task<PaymentRequestDto> CreateAsync(PaymentRequestCreationDto input);
    }
}