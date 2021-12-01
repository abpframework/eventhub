using Microsoft.EntityFrameworkCore;
using Payment.PaymentRequests;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Payment.EntityFrameworkCore
{
    [ConnectionStringName(PaymentDbProperties.ConnectionStringName)]
    public interface IPaymentDbContext : IEfCoreDbContext
    {
        DbSet<PaymentRequest> PaymentRequests { get; }
    }
}