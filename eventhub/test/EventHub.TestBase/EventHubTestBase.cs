using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Testing;
using Volo.Abp.Users;

namespace EventHub
{
    /* All test classes are derived from this class, directly or indirectly.
     */
    public abstract class EventHubTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
        where TStartupModule : IAbpModule
    {
        protected ICurrentUser CurrentUser { get; }

        public EventHubTestBase()
        {
            CurrentUser = GetRequiredService<ICurrentUser>();
        }

        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }

        protected virtual Task WithUnitOfWorkAsync(Func<Task> func)
        {
            return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
        }

        protected virtual async Task WithUnitOfWorkAsync(AbpUnitOfWorkOptions options, Func<Task> action)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

                using (var uow = uowManager.Begin(options))
                {
                    try
                    {
                        await action();
                        await uow.CompleteAsync();
                    }
                    catch
                    {
                        await uow.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        protected virtual Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
        {
            return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
        }

        protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(AbpUnitOfWorkOptions options, Func<Task<TResult>> func)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

                using (var uow = uowManager.Begin(options))
                {
                    try
                    {
                        var result = await func();
                        await uow.CompleteAsync();
                        return result;
                    }
                    catch
                    {
                        await uow.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        protected virtual async Task<Organization> GetOrganizationOrNullAsync(string name)
        {
            var organizationRepository = GetRequiredService<IRepository<Organization, Guid>>();
            return await WithUnitOfWorkAsync(async () =>
                {
                    return await organizationRepository.FirstOrDefaultAsync(o => o.Name == name);
                }
            );
        }
    }
}
