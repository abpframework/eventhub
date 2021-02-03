using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using EventHub.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
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
            var userRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<AppUser, Guid>>();
            var eventRegistrationRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<EventRegistration, Guid>>();
            
            var eventQueryable = await eventRepository.GetQueryableAsync();
            var userQueryable = await userRepository.GetQueryableAsync();
            var eventRegistrationQueryable = await eventRegistrationRepository.GetQueryableAsync();

            var thirtyOneMinutesAfter = DateTime.Now.AddMinutes(31);
            var twentyNineMinutesAfter = DateTime.Now.AddMinutes(29);

            var eventsStartingAfterThirtyMinutes = await eventQueryable.Where(x =>
                x.IsRemindingEmailSent == false && 
                x.StartTime < thirtyOneMinutesAfter &&
                x.StartTime > twentyNineMinutesAfter)
                .ToListAsync();

            foreach (var @event in eventsStartingAfterThirtyMinutes)
            {
                var usersToBeNotified = await (from eventRegistration in eventRegistrationQueryable
                    join user in userQueryable on eventRegistration.UserId equals user.Id
                    where eventRegistration.EventId == @event.Id
                    select user).ToListAsync();

                @event.IsRemindingEmailSent = true;

                try
                {
                    await eventReminderNotifier.NotifyAsync(@event, usersToBeNotified);
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