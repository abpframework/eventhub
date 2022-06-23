using System;
using System.Threading.Tasks;
using EventHub.Emailing;
using EventHub.Options;
using EventHub.Users;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.TextTemplating;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly EventHubUrlOptions _urlOptions;

        public EventRegistrationNotifier(
            IEmailSender emailSender, 
            ITemplateRenderer templateRenderer,
            IOptions<EventHubUrlOptions> urlOptions)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _urlOptions = urlOptions.Value;
        }
        
        public async Task NotifyAsync(
            Event @event, 
            IdentityUser user)
        {
            if (user is null || @event is null)
            {
                return;
            }

            var model = new
            {
                Title = @event.Title,
                Description = @event.Description.TruncateWithPostfix(250, "..."),
                Url = @event.Url,
                FullNameOrUserName = user.GetFullNameOrUsername(),
                ThumbnailUrl = $"{_urlOptions.Api.EnsureEndsWith('/')}api/eventhub/event/cover-image/{@event.Id}",
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                Location = @event.IsOnline ? "Online" : $"{@event.City}, {@event.CountryName}"
            };

            await _emailSender.QueueAsync(
                user.Email,
                "Your event registration has been received!",
                await _templateRenderer.RenderAsync(EmailTemplates.EventRegistration, model)
            );
        }
    }
}