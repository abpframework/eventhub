using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Emailing;
using EventHub.Events.Registrations;
using EventHub.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Linq;
using Volo.Abp.TextTemplating;

namespace EventHub.Events
{
    public class EventReminderNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly IAsyncQueryableExecuter _asyncExecuter;

        public EventReminderNotifier(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IRepository<AppUser, Guid> userRepository,
            IRepository<EventRegistration, Guid> eventRegistrationRepository,
            IAsyncQueryableExecuter asyncExecuter)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _userRepository = userRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _asyncExecuter = asyncExecuter;
        }

        public async Task NotifyAsync(Event @event)
        {
            if (@event is null)
            {
                return;
            }

            var userQueryable = await _userRepository.GetQueryableAsync();
            var registrationQueryable = await _eventRegistrationRepository.GetQueryableAsync();

            var userQuery = from eventRegistration in registrationQueryable
                join user in userQueryable on eventRegistration.UserId equals user.Id
                where eventRegistration.EventId == @event.Id
                select user;

            var users = await _asyncExecuter.ToListAsync(userQuery);

            foreach (var user in users)
            {
                var templateModel = new
                {
                    UserName = user.GetFullNameOrUsername(),
                    Title = @event.Title,
                    StartTime = @event.StartTime,
                    EndTime = @event.EndTime,
                    Url = @event.Url
                };

                await _emailSender.QueueAsync(
                    user.Email,
                    "The event has last thirty minutes to start!",
                    await _templateRenderer.RenderAsync(EmailTemplates.EventReminder, templateModel)
                );
            }
        }
    }
}