using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventHub.Data;
using Volo.Abp.DependencyInjection;

namespace EventHub.EntityFrameworkCore
{
    public class EntityFrameworkCoreEventHubDbSchemaMigrator
        : IEventHubDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreEventHubDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the EventHubMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<EventHubMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}