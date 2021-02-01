using System.Threading.Tasks;

namespace EventHub.Data
{
    public interface IEventHubDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
