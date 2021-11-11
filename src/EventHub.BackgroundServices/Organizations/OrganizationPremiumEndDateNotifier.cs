using System;
using System.Threading.Tasks;
using EventHub.Emailing;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.TextTemplating;

namespace EventHub.Organizations
{
    public class OrganizationPremiumEndDateNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly ILogger<OrganizationPremiumEndDateNotifier> _logger;

        public OrganizationPremiumEndDateNotifier(
            IEmailSender emailSender, 
            ITemplateRenderer templateRenderer,
            IRepository<IdentityUser, Guid> userRepository,
            ILogger<OrganizationPremiumEndDateNotifier> logger)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _userRepository = userRepository;
            _logger = logger;
        }
        
        public async Task NotifyAsync(Organization organization)
        {
            if (organization is null || organization.IsSendPremiumReminderEmail || !organization.IsPremium)
            {
                return;
            }

            var user = await _userRepository.FindAsync(organization.OwnerUserId);
            if (user is null)
            {
                _logger.LogError("(OrganizationPremiumEndDateNotifier) User not found!");
                return;
            }
            
            var templateModel = new
            {
                OrganizationName = organization.Name,
                EndDate = organization.PremiumEndDate!.Value.ToShortDateString()
            };

            await _emailSender.SendAsync(
                user.Email,
                "EventHub: Your Premium Organization is expiring!",
                await _templateRenderer.RenderAsync(EmailTemplates.PremiumEndDateReminder, templateModel)
            );
        }
    }
}