using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Payment.PaymentRequests
{
	[RemoteService(Name = PaymentRemoteServiceConsts.RemoteServiceName)]
    [Area("payment")]
    [Route("api/payment/requests")]
    public class PaymentRequestController : PaymentController, IPaymentRequestAppService
    {
        protected IPaymentRequestAppService PaymentRequestAppService { get; }

        public PaymentRequestController(IPaymentRequestAppService paymentRequestAppService)
        {
            PaymentRequestAppService = paymentRequestAppService;
        }

        [HttpPost]
        [Route("complete/{token}")]
        public Task<PaymentRequestDto> CompleteAsync([FromRoute] string token)
        {
            return PaymentRequestAppService.CompleteAsync(token);
        }

        [HttpPost]
        public Task<PaymentRequestDto> CreateAsync(PaymentRequestCreationDto input)
        {
            return PaymentRequestAppService.CreateAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public Task<PaymentRequestDto> GetAsync(Guid id)
        {
            return PaymentRequestAppService.GetAsync(id);
        }

        [HttpPost]
        [Route("start")]
        public Task<StartPaymentResultDto> StartPaymentAsync(StartPaymentDto input)
        {
            return PaymentRequestAppService.StartPaymentAsync(input);
        }
    }
}
