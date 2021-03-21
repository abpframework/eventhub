using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace EventHub.Events
{
    public class EventReminderWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public EventReminderWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
            : base(timer, serviceScopeFactory)
        {
            Timer.Period = 60_000;
        }

        [UnitOfWork(isTransactional: false)]
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var eventReminderNotifier = workerContext.ServiceProvider.GetRequiredService<EventReminderNotifier>();
            var eventRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Event, Guid>>();
            var asyncExecuter = workerContext.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();

            var thirtyMinutesAfter = DateTime.Now.AddMinutes(30);
            var oneMinutesAfter = DateTime.Now.AddMinutes(1);

            var queryable = await eventRepository.GetQueryableAsync();
            var query = queryable.Where(x =>
                x.IsRemindingEmailSent == false &&
                x.StartTime <= thirtyMinutesAfter &&
                x.StartTime >= oneMinutesAfter
            );

            var events = await asyncExecuter.ToListAsync(query);

            foreach (var @event in events)
            {
                try
                {
                    await eventReminderNotifier.NotifyAsync(@event);
                    @event.IsRemindingEmailSent = true;
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