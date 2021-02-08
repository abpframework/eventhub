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
    public class NewEventDetectorWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public NewEventDetectorWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) 
            : base(timer, serviceScopeFactory)
        {
            Timer.Period = 60_000;
        }

        [UnitOfWork]
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var newEventNotifier = workerContext.ServiceProvider.GetRequiredService<NewEventNotifier>();
            var eventRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Event, Guid>>();
            var asyncExecuter = workerContext.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();

            var eventQueryable = await eventRepository.GetQueryableAsync();

            var eventQuery = eventQueryable.Where(x => x.IsEmailSentToMembers == false);
            
            var newEvents = await asyncExecuter.ToListAsync(eventQuery);

            foreach (var @event in newEvents)
            {
                try
                {
                    await newEventNotifier.NotifyAsync(@event);
                    @event.IsEmailSentToMembers = true;
                }
                catch (Exception e)
                {
                    @event.IsEmailSentToMembers = false;
                    Logger.LogError($"An error occurred while sending an email to the members of the organization after the new event was created. Error message: {e.Message}");
                }
            }
        }
    }
}