using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Organizations;
using EventHub.Web.Pages.Organizations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Payment.PaymentRequests;
using Payment.Web.PaymentRequest;
using Volo.Abp.Authorization;
using Volo.Abp.Users;

namespace EventHub.Web.Pages
{
    public class Pricing : EventHubPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)] 
        public string OrganizationName { get; set; }

        [HiddenInput]
        [BindProperty]
        public OrganizationPlanType TargetPlanToUpgrade { get; set; }
        
        [ItemNotNull]
        public List<PlanInfoDefinitionDto> PlanInfos { get; private set; }
        
        public OrganizationProfileDto Organization { get; private set; }

        private readonly IOrganizationAppService _organizationAppService;
        private readonly IPaymentRequestAppService _paymentRequestAppService;
        private readonly IPaymentUrlBuilder _paymentUrlBuilder;

        public Pricing(
            IOrganizationAppService organizationAppService,
            IPaymentRequestAppService paymentRequestAppService,
            IPaymentUrlBuilder paymentUrlBuilder)
        {
            _organizationAppService = organizationAppService;
            _paymentRequestAppService = paymentRequestAppService;
            _paymentUrlBuilder = paymentUrlBuilder;
            PlanInfos = new List<PlanInfoDefinitionDto>();
        }

        public async Task OnGetAsync()
        {
            Organization = await _organizationAppService.GetProfileAsync(OrganizationName);
            if (CurrentUser.UserName != Organization.OwnerUserName)
            {
                throw new AbpAuthorizationException();
            }

            PlanInfos = await _organizationAppService.GetPlanInfosAsync();
        }

        public async Task<IActionResult> OnPostUpgradeAsync()
        {
            var organization = await GetOrganizationProfileAsync();
            PlanInfos = await _organizationAppService.GetPlanInfosAsync();
            var plan = PlanInfoHelper.GetPlan(TargetPlanToUpgrade, PlanInfos);
            if (plan is null || !plan.IsActive)
            {
                Alerts.Danger("Premium plan is currently inactive!");
                return Page();
            }
            
            var paymentRequest = await CreatePaymentRequestAsync(plan, organization);

            return Redirect(_paymentUrlBuilder.BuildCheckoutUrl(paymentRequest.Id).AbsoluteUri);
        }
        
        public async Task<IActionResult> OnPostExtendAsync()
        {
            var organization = await GetOrganizationProfileAsync();

            PlanInfos = await _organizationAppService.GetPlanInfosAsync();

            var plan = PlanInfoHelper.GetPlan(TargetPlanToUpgrade, PlanInfos);
            if (plan is null || !plan.IsActive)
            {
                Alerts.Danger("Premium plan is currently inactive!");
                return Page();
            }
            
            var paymentRequest = await CreatePaymentRequestAsync(plan, organization, true);

            return Redirect(_paymentUrlBuilder.BuildCheckoutUrl(paymentRequest.Id).AbsoluteUri);
        }

        private async Task<OrganizationProfileDto> GetOrganizationProfileAsync()
        {
            var organization = await _organizationAppService.GetProfileAsync(OrganizationName);
            if (organization.OwnerUserName != CurrentUser.UserName)
            {
                throw new AbpAuthorizationException();
            }

            return organization;
        }

        private async Task<PaymentRequestDto> CreatePaymentRequestAsync(
            PlanInfoDefinitionDto plan,
            OrganizationProfileDto organization, 
            bool isExtend = false)
        {
            var paymentRequest = await _paymentRequestAppService.CreateAsync(new PaymentRequestCreationDto
            {
                CustomerId = CurrentUser.GetId().ToString(),
                Price = plan.Price,
                ProductId = PlanInfoHelper.GetProductId(plan, isExtend),
                ProductName = PlanInfoHelper.GetProductName(plan, organization.Name, isExtend),
                ExtraProperties =
                {
                    {
                        nameof(OrganizationPaymentRequestExtraParameterConfiguration),
                        new OrganizationPaymentRequestExtraParameterConfiguration
                        {
                            OrganizationName = OrganizationName,
                            PremiumPeriodAsMonth = plan.OnePaidEnrollmentPeriodAsMonth,
                            IsExtend = isExtend,
                            TargetPlanType = TargetPlanToUpgrade
                        }
                    }
                }
            });
            
            return paymentRequest;
        }
    }
}
