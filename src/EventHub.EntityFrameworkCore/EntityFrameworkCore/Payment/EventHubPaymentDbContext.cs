using Microsoft.EntityFrameworkCore;
using Payment;
using Payment.EntityFrameworkCore;
using Payment.PaymentRequests;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace EventHub.EntityFrameworkCore.Payment
{
    /*
     * This class replaces the IPaymentDbContext (defined by the Payment module)
     * using the [ReplaceDbContext] attribute and implementing the IPaymentDbContext interface.
     * It also declares the [ConnectionStringName] attribute to use the Payment connection string name
     * instead of the Default in appsettings.json files.
     * Finally, it calls modelBuilder.ConfigurePayment() extension method of the Payment module
     * to configure the database mappings. 
     */
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