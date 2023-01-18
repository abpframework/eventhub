using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EventHub.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class EventHubDbContextFactory : IDesignTimeDbContextFactory<EventHubDbContext>
    {
        public EventHubDbContext CreateDbContext(string[] args)
        {
            EventHubEfCoreEntityExtensionMappings.Configure();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<EventHubDbContext>()
                .UseNpgsql(configuration.GetConnectionString("Default"));

            return new EventHubDbContext(builder.Options);
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
