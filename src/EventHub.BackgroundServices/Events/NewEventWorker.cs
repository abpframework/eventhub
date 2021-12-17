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

namespace EventHub.Events
{
    public class NewEventWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public NewEventWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory)
            : base(
                timer,
                serviceScopeFactory)
        {
            Timer.Period = 60_000;
        }

        [UnitOfWork(isTransactional: false)]
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var newEventNotifier = workerContext.ServiceProvider.GetRequiredService<NewEventNotifier>();
            var eventRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Event, Guid>>();
            var asyncExecuter = workerContext.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();
            var clock = workerContext.ServiceProvider.GetRequiredService<IClock>();

            var queryable = await eventRepository.GetQueryableAsync();
            var newEvents = await asyncExecuter.ToListAsync(
                queryable.Where(x => x.IsEmailSentToMembers == false && !x.IsDraft)
            );

            var fiveMinutesLater = clock.Now.AddMinutes(5);
            foreach (var @event in newEvents)
            {
                try
                {
                    if (@event.StartTime >= fiveMinutesLater)
                    {
                        await newEventNotifier.NotifyAsync(@event);
                    }

                    @event.IsEmailSentToMembers = true;
                    await eventRepository.UpdateAsync(@event);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }
    }
}
