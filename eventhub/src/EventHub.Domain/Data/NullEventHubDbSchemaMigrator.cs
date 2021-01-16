using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EventHub.Data
{
    /* This is used if database provider does't define
     * IEventHubDbSchemaMigrator implementation.
     */
    public class NullEventHubDbSchemaMigrator : IEventHubDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}