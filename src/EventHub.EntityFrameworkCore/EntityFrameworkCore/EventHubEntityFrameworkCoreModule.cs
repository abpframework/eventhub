using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using System;
using EventHub.EntityFrameworkCore.Payment;
using Payment.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;

namespace EventHub.EntityFrameworkCore
{
    [DependsOn(
        typeof(EventHubDomainModule),
        typeof(AbpIdentityEntityFrameworkCoreModule),
        typeof(AbpIdentityServerEntityFrameworkCoreModule),
        typeof(AbpPermissionManagementEntityFrameworkCoreModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCorePostgreSqlModule),
        typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(BlobStoringDatabaseEntityFrameworkCoreModule),
        typeof(PaymentEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule)
        )]
    public class EventHubEntityFrameworkCoreModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            EventHubEfCoreEntityExtensionMappings.Configure();

            //allows to use DateTime with timezone (by default)
            //See: https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<EventHubDbContext>(options =>
            {
                options.AddDefaultRepositories();
            });
            
            /* Registering the payment dbcontext and replacing the module's dbcontext */
            context.Services.AddAbpDbContext<EventHubPaymentDbContext>();

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseNpgsql();
                
                /* While the default db provider is PostgreSQl (because of `options.UseNpgsql()` above),
                 * We are configuring to use SQL Server for the payment database
                 */
                options.Configure<EventHubPaymentDbContext>(opts =>
                {
                    opts.UseSqlServer();
                });
            });
        }
    }
}
