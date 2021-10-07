using System;
using Volo.Abp.Domain.Repositories;

namespace Payment.PaymentRequests
{
    public interface IPaymentRequestRepository : IRepository<PaymentRequest, Guid>
    {
        
    }
}