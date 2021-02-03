using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Emailing;
using EventHub.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;

namespace EventHub.Events
{
    public class EventReminderNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;

        public EventReminderNotifier(
            IEmailSender emailSender, 
            ITemplateRenderer templateRenderer)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
        }
        
        public async Task NotifyAsync(
            Event @event, 
            IEnumerable<AppUser> users)
        {
            if (users is null || @event is null)
            {
                return;
            }

            foreach (var user in users)
            {
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
                    "The event has last thirty minutes to start!",
                    await _templateRenderer.RenderAsync(EmailTemplates.EventReminder, model)
                );   
            }
        }
    }
}