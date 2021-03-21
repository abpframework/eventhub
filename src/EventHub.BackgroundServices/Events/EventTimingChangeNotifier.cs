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
            IRepository<AppUser, Guid> userRepository
        )
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

            var eventRegistrationQueryable = await _eventRegistrationRepository.GetQueryableAsync();
            var query = eventRegistrationQueryable
                .Where(x => x.EventId == @event.Id && !x.IsTimingChangeEmailSent);
            
            var eventRegistrations = await _asyncExecuter.ToListAsync(query);

            foreach (var eventRegistration in eventRegistrations)
            {
                var user = await _userRepository.FindAsync(eventRegistration.UserId);

                if (user is null)
                {
                    continue;
                }

                var model = new
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
                    await _templateRenderer.RenderAsync(EmailTemplates.EventTimingChanged, model)
                );

                eventRegistration.IsTimingChangeEmailSent = true;
            }

            await _eventRegistrationRepository.UpdateManyAsync(eventRegistrations, autoSave: true);
        }
    }
}