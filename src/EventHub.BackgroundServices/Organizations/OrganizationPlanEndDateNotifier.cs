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
    public class OrganizationPlanEndDateNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly ILogger<OrganizationPlanEndDateNotifier> _logger;

        public OrganizationPlanEndDateNotifier(
            IEmailSender emailSender, 
            ITemplateRenderer templateRenderer,
            IRepository<IdentityUser, Guid> userRepository,
            ILogger<OrganizationPlanEndDateNotifier> logger)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _userRepository = userRepository;
            _logger = logger;
        }
        
        public async Task NotifyAsync(Organization organization)
        {
            if (organization is null || organization.IsSendPaidEnrollmentReminderEmail || organization.PlanType != OrganizationPlanType.Free)
            {
                return;
            }

            var user = await _userRepository.FindAsync(organization.OwnerUserId);
            if (user is null)
            {
                _logger.LogError("(OrganizationPaidEnrollmentEndDateNotifier) User not found!");
                return;
            }
            
            var templateModel = new
            {
                OrganizationName = organization.Name,
                EndDate = organization.PaidEnrollmentEndDate!.Value.ToShortDateString()
            };

            await _emailSender.SendAsync(
                user.Email,
                "EventHub: Your" + organization.PlanType +  "Organization is expiring!",
                await _templateRenderer.RenderAsync(EmailTemplates.PaidEnrollmentEndDateReminder, templateModel)
            );
        }
    }
}
