using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace EventHub.Events
{
    public class EventTimingChangeWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public EventTimingChangeWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            Timer.Period = 60000; //1 minute
        }

        [UnitOfWork(isTransactional: false)]
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var eventTimingChangeNotifier = workerContext.ServiceProvider.GetRequiredService<EventTimingChangeNotifier>();
            var eventRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Event, Guid>>();
            var eventRegistrationRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<EventRegistration, Guid>>();
            var asyncExecuter = workerContext.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();

            var eventRegistrationQueryable = await eventRegistrationRepository.GetQueryableAsync();
            var query = eventRegistrationQueryable
                .Where(x => !x.IsTimingChangeEmailSent)
                .Select(x => x.EventId)
                .Distinct();
            
            var eventRegistrations = await asyncExecuter.ToListAsync(query);
           
            foreach (var eventRegistration in eventRegistrations)
            {
                try
                {
                    await eventTimingChangeNotifier.NotifyAsync(await eventRepository.FindAsync(eventRegistration));
                }
                catch (Exception e)
                {
                    Logger.LogError($"An error occurred while sending an email to the attendees of the event after the event time has changed. Error message: {e.Message}");
                }    
            }
        }
    }
}