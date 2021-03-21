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
    public class EventTimingChangeNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly IAsyncQueryableExecuter _asyncExecuter;
        private readonly IRepository<AppUser, Guid> _userRepository;

        public EventTimingChangeNotifier(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IRepository<EventRegistration, Guid> eventRegistrationRepository,
            IAsyncQueryableExecuter asyncExecuter,
            IRepository<AppUser, Guid> userRepository)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _eventRegistrationRepository = eventRegistrationRepository;
            _asyncExecuter = asyncExecuter;
            _userRepository = userRepository;
        }

        public async Task NotifyAsync(Event @event)
        {
            if (@event is null)
            {
                return;
            }
            
            //TODO: It will be more performant if we join to users instead of individually query (already done for EventReminderNotifier)

            var queryable = await _eventRegistrationRepository.GetQueryableAsync();
            var registrations = await _asyncExecuter.ToListAsync(
                queryable.Where(x => x.EventId == @event.Id && !x.IsTimingChangeEmailSent)
            );

            foreach (var registration in registrations)
            {
                var user = await _userRepository.FindAsync(registration.UserId);
                if (user is null)
                {
                    continue;
                }

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
                    "Event time has been changed!",
                    await _templateRenderer.RenderAsync(EmailTemplates.EventTimingChanged, templateModel)
                );

                registration.IsTimingChangeEmailSent = true;
            }

            await _eventRegistrationRepository.UpdateManyAsync(registrations, autoSave: true);
        }
    }
}