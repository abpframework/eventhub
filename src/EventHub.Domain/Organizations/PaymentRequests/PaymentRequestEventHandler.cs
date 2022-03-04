using System;
using System.Threading.Tasks;
using EventHub.Emailing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Payment.PaymentRequests;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.TextTemplating;

namespace EventHub.Organizations.PaymentRequests
{
    public class PaymentRequestEventHandler : IDistributedEventHandler<PaymentRequestCompletedEto>,
        IDistributedEventHandler<PaymentRequestFailedEto>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<PaymentRequest, Guid> _paymentRequestRepository;
        private readonly ILogger<PaymentRequestEventHandler> _logger;
        
        public PaymentRequestEventHandler(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IRepository<IdentityUser, Guid> userRepository,
            IRepository<Organization, Guid> organizationRepository,
            IRepository<PaymentRequest, Guid> paymentRequestRepository,
            ILogger<PaymentRequestEventHandler> logger)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            _paymentRequestRepository = paymentRequestRepository;
            _logger = logger;
        }

        public async Task HandleEventAsync(PaymentRequestCompletedEto eventData)
        {
            var isExistExtraProperties = eventData.ExtraProperties.TryGetValue(nameof(OrganizationPaymentRequestExtraParameterConfiguration), out var ExtraProperties);
            if (!isExistExtraProperties || ExtraProperties is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, organization could not be upgrade because ExtraProperties do not exist!");
                return;
            }

            var paymentRequestProductExtraParameter = JsonConvert.DeserializeObject<OrganizationPaymentRequestExtraParameterConfiguration>(ExtraProperties.ToString());
            if (paymentRequestProductExtraParameter is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, organization could not be upgrade because the ExtraProperties cannot be deserialized!");
                return;
            }
            
            var payment = await _paymentRequestRepository.GetAsync(eventData.PaymentRequestId);
            
            var user = await _userRepository.FindAsync(Guid.Parse(payment.CustomerId));
            if (user is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, mail could not be sent because the user(UserId:{payment.CustomerId}) could not be found!");
                return;
            }

            var updatedOrganization = await UpgradeOrganizationAsync(paymentRequestProductExtraParameter);
            if (updatedOrganization is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, organization could not be upgrade! OrganizationName: {paymentRequestProductExtraParameter.OrganizationName}");
                return;
            }

            var templateModel = new
            {
                OrganizationName = paymentRequestProductExtraParameter.OrganizationName,
                PlanInfo = paymentRequestProductExtraParameter.IsExtend ? "extended" : "upgraded",
                Price = payment.Price,
                Currency = payment.Currency
            };
            
            await _emailSender.SendAsync(
                user.Email,
                "EventHub: Thank You",
                await _templateRenderer.RenderAsync(EmailTemplates.PaymentRequestCompleted, templateModel)
            );
        }

        public async Task HandleEventAsync(PaymentRequestFailedEto eventData)
        {
            var isExistExtraProperties = eventData.ExtraProperties.TryGetValue(nameof(OrganizationPaymentRequestExtraParameterConfiguration), out var ExtraProperties);
            if (!isExistExtraProperties || ExtraProperties is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, mail could not be sent because ExtraProperties do not exist!");
                return;
            }

            var paymentRequestProductExtraParameter = JsonConvert.DeserializeObject<OrganizationPaymentRequestExtraParameterConfiguration>(ExtraProperties.ToString());
            if (paymentRequestProductExtraParameter is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, mail could not be sent because the ExtraProperties cannot be deserialized!");
                return;
            }

            var payment = await _paymentRequestRepository.GetAsync(eventData.PaymentRequestId);
            var user = await _userRepository.FindAsync(Guid.Parse(payment.CustomerId));
            if (user is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, mail could not be sent because the user(UserId:{payment.CustomerId}) could not be found!");
                return;
            }
            
            var templateModel = new
            {
                OrganizationName = paymentRequestProductExtraParameter.OrganizationName,
                LicenseType = OrganizationPlanType.Premium,
                FailReason = eventData.FailReason.IsNullOrWhiteSpace() ? "An unknown error has occurred. Please contact us." : eventData.FailReason
            };
            
            await _emailSender.QueueAsync(
                user.Email,
                "EventHub: Payment Failed",
                await _templateRenderer.RenderAsync(EmailTemplates.PaymentRequestFailed, templateModel)
            );
        }
        
        private async Task<Organization> UpgradeOrganizationAsync(OrganizationPaymentRequestExtraParameterConfiguration organizationPaymentRequestExtraParameter)
        {
            var organization = await _organizationRepository.FindAsync(o => o.Name == organizationPaymentRequestExtraParameter.OrganizationName);
            if (organization is null)
            {
                return null;
            }

            if (organizationPaymentRequestExtraParameter.IsExtend)
            {
                organization.UpgradeToPlanType(organizationPaymentRequestExtraParameter.TargetPlanType,
                    organization.PaidEnrollmentEndDate!.Value.AddMonths(organizationPaymentRequestExtraParameter.PremiumPeriodAsMonth!.Value));
            }
            else
            {
                organization.UpgradeToPlanType(organizationPaymentRequestExtraParameter.TargetPlanType, DateTime.Now.AddMonths(organizationPaymentRequestExtraParameter.PremiumPeriodAsMonth!.Value));
            }
            
            return await _organizationRepository.UpdateAsync(organization, true);
        }
    }
}
