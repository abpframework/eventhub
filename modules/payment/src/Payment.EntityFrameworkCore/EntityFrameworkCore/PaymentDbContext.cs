using Microsoft.EntityFrameworkCore;
using Payment.PaymentRequests;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Payment.EntityFrameworkCore
{
    [ConnectionStringName(PaymentDbProperties.ConnectionStringName)]
    public class PaymentDbContext : AbpDbContext<PaymentDbContext>, IPaymentDbContext
    {
        public DbSet<PaymentRequest> PaymentRequests { get; set; }

        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigurePayment();
        }
    }
}