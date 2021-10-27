using System.Threading.Tasks;
using EventHub.Organizations;
using EventHub.Web.PaymentRequests;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        
        [CanBeNull] 
        public PremiumPlanInfoOptions PremiumPlanInfo { get; private set; }
        
        public OrganizationProfileDto Organization { get; private set; }

        private readonly IOrganizationAppService _organizationAppService;
        private readonly IPaymentRequestAppService _paymentRequestAppService;
        private readonly IPaymentUrlBuilder _paymentUrlBuilder;
        private readonly IOptionsSnapshot<PremiumPlanInfoOptions> _premiumPlanInfoOptionsSnapshot;

        public Pricing(
            IOrganizationAppService organizationAppService,
            IPaymentRequestAppService paymentRequestAppService,
            IPaymentUrlBuilder paymentUrlBuilder,
            IOptionsSnapshot<PremiumPlanInfoOptions> premiumPlanInfoOptionsSnapshot)
        {
            _organizationAppService = organizationAppService;
            _paymentRequestAppService = paymentRequestAppService;
            _paymentUrlBuilder = paymentUrlBuilder;
            _premiumPlanInfoOptionsSnapshot = premiumPlanInfoOptionsSnapshot;
        }

        public async Task OnGetAsync()
        {
            Organization = await _organizationAppService.GetProfileAsync(OrganizationName);
            if (CurrentUser.UserName != Organization.OwnerUserName)
            {
                throw new AbpAuthorizationException();
            }
            
            if (_premiumPlanInfoOptionsSnapshot.Value.IsActive)
            {
                PremiumPlanInfo = _premiumPlanInfoOptionsSnapshot.Value;
            }

        }

        public async Task<IActionResult> OnPostUpgradeAsync()
        {
            var organization = await GetOrganizationProfileAsync();

            var plan =  _premiumPlanInfoOptionsSnapshot.Value;
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

            var plan =  _premiumPlanInfoOptionsSnapshot.Value;
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
            PremiumPlanInfoOptions plan,
            OrganizationProfileDto organization, 
            bool isExtend = false)
        {
            var paymentRequest = await _paymentRequestAppService.CreateAsync(new PaymentRequestCreationDto
            {
                CustomerId = CurrentUser.GetId().ToString(),
                Price = plan.Price,
                ProductId = PremiumPlanInfoOptions.GetProductId(plan, isExtend),
                ProductName = PremiumPlanInfoOptions.GetProductName(plan, organization.Name, isExtend),
                ExtraProperties =
                {
                    {
                        nameof(OrganizationPaymentRequestExtraParameterConfiguration),
                        new OrganizationPaymentRequestExtraParameterConfiguration
                        {
                            OrganizationName = OrganizationName,
                            PremiumPeriodAsMonth = plan.OnePremiumPeriodAsMonth,
                            IsExtend = isExtend
                        }
                    }
                }
            });
            
            return paymentRequest;
        }
    }
}