using Microsoft.EntityFrameworkCore;
using Payment;
using Payment.EntityFrameworkCore;
using Payment.PaymentRequests;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace EventHub.EntityFrameworkCore.Payment
{
    [ReplaceDbContext(typeof(IPaymentDbContext))]
    [ConnectionStringName(PaymentDbProperties.ConnectionStringName)]
    public class EventHubPaymentDbContext : AbpDbContext<EventHubPaymentDbContext>, IPaymentDbContext
    {
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        
        public EventHubPaymentDbContext(
            DbContextOptions<EventHubPaymentDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigurePayment();
        }
    }
}