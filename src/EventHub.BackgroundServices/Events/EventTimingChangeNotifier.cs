using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Emailing;
using EventHub.Events.Registrations;
using EventHub.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Linq;
using Volo.Abp.TextTemplating;

namespace EventHub.Events
{
    public class EventTimingChangeNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IAsyncQueryableExecuter _asyncExecuter;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;

        public EventTimingChangeNotifier(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IAsyncQueryableExecuter asyncExecuter,
            IRepository<IdentityUser, Guid> userRepository,
            IRepository<EventRegistration, Guid> eventRegistrationRepository)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _asyncExecuter = asyncExecuter;
            _userRepository = userRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
        }

        public async Task NotifyAsync(Event @event)
        {
            if (@event is null || @event.IsTimingChangeEmailSent || !@event.IsDraft)
            {
                return;
            }

            var userQueryable = await _userRepository.GetQueryableAsync();
            var registrationQueryable = await _eventRegistrationRepository.GetQueryableAsync();

            var userQuery = from eventRegistration in registrationQueryable
                join user in userQueryable on eventRegistration.UserId equals user.Id
                where eventRegistration.EventId == @event.Id && !@event.IsDraft
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
                    "Event time has been changed!",
                    await _templateRenderer.RenderAsync(EmailTemplates.EventTimingChanged, templateModel)
                );
            }
        }
    }
}
