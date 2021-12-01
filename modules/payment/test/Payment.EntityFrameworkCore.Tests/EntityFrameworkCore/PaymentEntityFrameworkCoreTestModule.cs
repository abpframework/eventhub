using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

namespace Payment.EntityFrameworkCore
{
    [DependsOn(
        typeof(PaymentTestBaseModule),
        typeof(PaymentEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCoreSqliteModule)
        )]
    public class PaymentEntityFrameworkCoreTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var sqliteConnection = CreateDatabaseAndGetConnection();

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(abpDbContextConfigurationContext =>
                {
                    abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection);
                });
            });
        }
        
        private static SqliteConnection CreateDatabaseAndGetConnection()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            new PaymentDbContext(
                new DbContextOptionsBuilder<PaymentDbContext>().UseSqlite(connection).Options
            ).GetService<IRelationalDatabaseCreator>().CreateTables();
            
            return connection;
        }
    }
}
