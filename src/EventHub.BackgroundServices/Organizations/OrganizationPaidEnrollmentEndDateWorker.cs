using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.Threading;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace EventHub.Organizations
{
    public class OrganizationPaidEnrollmentEndDateWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public OrganizationPaidEnrollmentEndDateWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            Timer.Period = (int)TimeSpan.FromDays(1).TotalMilliseconds;
        }

        [UnitOfWork(isTransactional: false)]
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var organizationPaidEnrollmentEndDateNotifier = workerContext.ServiceProvider.GetRequiredService<OrganizationPlanEndDateNotifier>();
            var organizationRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Organization, Guid>>();
            var asyncExecuter = workerContext.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();
            var clock = workerContext.ServiceProvider.GetRequiredService<IClock>();

            await SendPremiumReminderEmail(organizationRepository, organizationPaidEnrollmentEndDateNotifier, asyncExecuter, clock);
            await UpdatePremiumExpiredOrganization(organizationRepository, asyncExecuter, clock);
        }

        private async Task SendPremiumReminderEmail(
            IRepository<Organization, Guid> organizationRepository,
            OrganizationPlanEndDateNotifier organizationPlanEndDateNotifier, 
            IAsyncQueryableExecuter asyncExecuter,
            IClock clock)
        {
            var oneMonthLater = clock.Now.AddMonths(1);
            var queryable = await organizationRepository.GetQueryableAsync();
            var query = queryable.Where(x =>
                x.IsSendPaidEnrollmentReminderEmail == false &&
                x.PaidEnrollmentEndDate <= oneMonthLater &&
                x.PlanType != OrganizationPlanType.Free
            );

            var organizations = await asyncExecuter.ToListAsync(query);

            foreach (var organization in organizations)
            {
                try
                {
                    if (organization.PaidEnrollmentEndDate >= clock.Now)
                    {
                        await organizationPlanEndDateNotifier.NotifyAsync(organization);
                    }

                    organization.IsSendPaidEnrollmentReminderEmail = true;
                    await organizationRepository.UpdateAsync(organization);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }
        
        private async Task UpdatePremiumExpiredOrganization(IRepository<Organization, Guid> organizationRepository, IAsyncQueryableExecuter asyncExecuter, IClock clock)
        {
            var queryable = await organizationRepository.GetQueryableAsync();
            var query = queryable.Where(x =>
                x.PaidEnrollmentEndDate > clock.Now.Date &&
                x.PlanType != OrganizationPlanType.Free
            );

            var organizations = await asyncExecuter.ToListAsync(query);

            foreach (var organization in organizations)
            {
                try
                {
                    organization.SetFreeToPlanType();
                    organization.IsSendPaidEnrollmentReminderEmail = false;
                    await organizationRepository.UpdateAsync(organization);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }
    }
}
