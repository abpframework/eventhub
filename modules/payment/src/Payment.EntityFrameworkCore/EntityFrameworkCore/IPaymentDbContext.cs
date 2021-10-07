using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Payment.EntityFrameworkCore
{
    [ConnectionStringName(PaymentDbProperties.ConnectionStringName)]
    public interface IPaymentDbContext : IEfCoreDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
    }
}