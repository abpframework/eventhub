using System.Threading.Tasks;
using EventHub.Emailing;
using EventHub.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;

        public EventRegistrationNotifier(
            IEmailSender emailSender, 
            ITemplateRenderer templateRenderer)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
        }
        
        public async Task NotifyAsync(
            Event @event, 
            AppUser user)
        {
            if (user is null || @event is null)
            {
                return;
            }

            var model = new
            {
                Title = @event.Title,
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                Url = @event.Url
            };

            await _emailSender.QueueAsync(
                user.Email,
                "Your event registration has been received!",
                await _templateRenderer.RenderAsync(EmailTemplates.EventRegistration, model)
            );
        }
    }
}