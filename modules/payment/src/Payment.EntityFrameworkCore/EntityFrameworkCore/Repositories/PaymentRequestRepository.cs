using System;
using Payment.PaymentRequests;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Payment.EntityFrameworkCore.Repositories
{
    public class PaymentRequestRepository : EfCoreRepository<IPaymentDbContext, PaymentRequest,Guid>, IPaymentRequestRepository
    {
        public PaymentRequestRepository(IDbContextProvider<IPaymentDbContext> dbContextProvider) 
            : base(dbContextProvider)
        {
        }
    }
}