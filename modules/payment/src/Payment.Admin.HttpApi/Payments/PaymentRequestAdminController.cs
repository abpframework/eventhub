using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Payment.Admin.Payments
{
    [RemoteService(Name = PaymentAdminRemoteServiceConsts.RemoteServiceName)]
    [Area("paymentAdmin")]
    [Route("api/payment/requests")]
    public class PaymentRequestAdminController : PaymentAdminController, IPaymentRequestAdminAppService
    {
        protected IPaymentRequestAdminAppService PaymentRequestAdminAppService { get; }

        public PaymentRequestAdminController(IPaymentRequestAdminAppService paymentRequestAdminAppService)
        {
            PaymentRequestAdminAppService = paymentRequestAdminAppService;
        }
        
        [HttpGet]
        public Task<PagedResultDto<PaymentRequestWithDetailsDto>> GetListAsync(PaymentRequestGetListInput input)
        {
            return PaymentRequestAdminAppService.GetListAsync(input);
        }
    }
}