using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Emailing;
using EventHub.Organizations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payment.PaymentRequests;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.TextTemplating;

namespace EventHub.PaymentRequest
{
    public class PaymentRequestEventHandler : IDistributedEventHandler<PaymentRequestCompletedEto>,
        IDistributedEventHandler<PaymentRequestFailedEto>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<Payment.PaymentRequests.PaymentRequest, Guid> _paymentRequestRepository;
        private readonly IOptionsSnapshot<List<OrganizationPlanInfoOptions>> _organizationPlanInfoOptionsSnapshot;
        private readonly ILogger<PaymentRequestEventHandler> _logger;
        
        public PaymentRequestEventHandler(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IRepository<IdentityUser, Guid> userRepository,
            IRepository<Organization, Guid> organizationRepository,
            IRepository<Payment.PaymentRequests.PaymentRequest, Guid> paymentRequestRepository,
            IOptionsSnapshot<List<OrganizationPlanInfoOptions>> organizationPlanInfoOptionsSnapshot,
            ILogger<PaymentRequestEventHandler> logger)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            _paymentRequestRepository = paymentRequestRepository;
            _organizationPlanInfoOptionsSnapshot = organizationPlanInfoOptionsSnapshot;
            _logger = logger;
        }

        public async Task HandleEventAsync(PaymentRequestCompletedEto eventData)
        {
            var isExistExtraProperties = eventData.ExtraProperties.TryGetValue(nameof(PaymentRequestProductExtraParameterConfiguration), out var ExtraProperties);
            if (!isExistExtraProperties || ExtraProperties is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, organization could not be upgrade because ExtraProperties do not exist!");
                return;
            }

            var paymentRequestProductExtraParameter = JsonConvert.DeserializeObject<PaymentRequestProductExtraParameterConfiguration>(ExtraProperties.ToString());
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
            
            var organization = await _organizationRepository.FindAsync(o => o.Name == paymentRequestProductExtraParameter.OrganizationName);
            if (organization is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, organization could not be upgrade because the organization(OrganizationId:{paymentRequestProductExtraParameter.OrganizationName}) could not be found!");
                return;
            }

            var plan = _organizationPlanInfoOptionsSnapshot.Value.First(x => x.PlanType == paymentRequestProductExtraParameter.TargetPlanType);
            organization.UpgradeToPremium(DateTime.Now.AddMonths(plan.OnePremiumPeriodAsMonth));
            await _organizationRepository.UpdateAsync(organization, true);

            var templateModel = new
            {
                OrganizationName = paymentRequestProductExtraParameter.OrganizationName,
                Price = payment.Price,
                Currency = payment.Currency
            };
            
            await _emailSender.QueueAsync(
                user.Email,
                "EventHub: Thank You",
                await _templateRenderer.RenderAsync(EmailTemplates.PaymentRequestCompleted, templateModel)
            );
        }

        public async Task HandleEventAsync(PaymentRequestFailedEto eventData)
        {
            var isExistExtraProperties = eventData.ExtraProperties.TryGetValue(nameof(PaymentRequestProductExtraParameterConfiguration), out var ExtraProperties);
            if (!isExistExtraProperties || ExtraProperties is null)
            {
                _logger.LogCritical($"{eventData.PaymentRequestId} PaymentRequestId although payment was received, mail could not be sent because ExtraProperties do not exist!");
                return;
            }

            var paymentRequestProductExtraParameter = JsonConvert.DeserializeObject<PaymentRequestProductExtraParameterConfiguration>(ExtraProperties.ToString());
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
                LicenseType = paymentRequestProductExtraParameter.TargetPlanType,
                FailReason = eventData.FailReason
            };
            
            await _emailSender.QueueAsync(
                user.Email,
                "EventHub: Payment Failed",
                await _templateRenderer.RenderAsync(EmailTemplates.PaymentRequestCompleted, templateModel)
            );
        }
    }
}