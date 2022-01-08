using System;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Uow;
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
            return await WithUnitOfWorkAsync(
                () => organizationRepository.FirstOrDefaultAsync(o => o.Name == name)
            );
        }

        protected virtual async Task<Organization> GetOrganizationAsync(string name)
        {
            var organization = await GetOrganizationOrNullAsync(name);
            if (organization == null)
            {
                throw new EntityNotFoundException(typeof(Event), name);
            }

            return organization;
        }

        protected virtual async Task<Event> GetEventOrNullAsync(Guid id)
        {
            var organizationRepository = GetRequiredService<IRepository<Event, Guid>>();
            return await WithUnitOfWorkAsync(
                () => organizationRepository.FindAsync(id)
            );
        }

        protected virtual async Task<Event> GetEventAsync(Guid id)
        {
            var @event = await GetEventOrNullAsync(id);
            if (@event == null)
            {
                throw new EntityNotFoundException(typeof(Event), id);
            }

            return @event;
        }

        protected virtual async Task<IdentityUser> GetUserOrNullAsync(Guid id)
        {
            var userRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
            return await WithUnitOfWorkAsync(
                () => userRepository.FindAsync(id)
            );
        }

        protected virtual async Task<IdentityUser> GetUserAsync(Guid id)
        {
            var user = await GetUserOrNullAsync(id);
            if (user == null)
            {
                throw new EntityNotFoundException(typeof(IdentityUser), id);
            }

            return user;
        }
    }
}
