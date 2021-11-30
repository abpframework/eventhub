using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Payment;

namespace EventHub.EntityFrameworkCore.Payment
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class EventHubPaymentDbContextFactory : IDesignTimeDbContextFactory<EventHubPaymentDbContext>
    {
        public EventHubPaymentDbContext CreateDbContext(string[] args)
        {
            EventHubEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<EventHubPaymentDbContext>()
                .UseSqlServer(configuration.GetConnectionString(PaymentDbProperties.ConnectionStringName));

            return new EventHubPaymentDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../EventHub.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
