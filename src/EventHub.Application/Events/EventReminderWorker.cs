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

        [UnitOfWork]
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var eventReminderNotifier = workerContext.ServiceProvider.GetRequiredService<EventReminderNotifier>();
            var eventRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Event, Guid>>();
            var asyncExecuter = workerContext.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();

            var eventQueryable = await eventRepository.GetQueryableAsync();

            var thirtyOneMinutesAfter = DateTime.Now.AddMinutes(30);
            var oneMinutesAfter = DateTime.Now.AddMinutes(1);

            var eventQuery = eventQueryable.Where(x =>
                x.IsRemindingEmailSent == false &&
                x.StartTime <= thirtyOneMinutesAfter &&
                x.StartTime >= oneMinutesAfter);
            
            var events = await asyncExecuter.ToListAsync(eventQuery);

            foreach (var @event in events)
            {
                try
                {
                    await eventReminderNotifier.NotifyAsync(@event);
                    @event.IsRemindingEmailSent = true;
                }
                catch (Exception e)
                {
                    @event.IsRemindingEmailSent = false;
                    Logger.LogError($"An error occurred while sending reminder mail for {@event.Id} event. Error message: {e.Message}");
                }
            }
        }
    }
}