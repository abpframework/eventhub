using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Organizations;
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
        
        public List<OrganizationPlanInfoOptions> OrganizationPlanInfos { get; private set; }
        
        public OrganizationProfileDto Organization { get; private set; }

        private readonly IOrganizationAppService _organizationAppService;
        private readonly IPaymentRequestAppService _paymentRequestAppService;
        private readonly IPaymentUrlBuilder _paymentUrlBuilder;
        private readonly IOptionsSnapshot<List<OrganizationPlanInfoOptions>> _organizationPlanInfoOptionsSnapshot;

        public Pricing(
            IOrganizationAppService organizationAppService,
            IPaymentRequestAppService paymentRequestAppService,
            IPaymentUrlBuilder paymentUrlBuilder,
            IOptionsSnapshot<List<OrganizationPlanInfoOptions>> organizationPlanInfoOptionsSnapshot)
        {
            _organizationAppService = organizationAppService;
            _paymentRequestAppService = paymentRequestAppService;
            _paymentUrlBuilder = paymentUrlBuilder;
            _organizationPlanInfoOptionsSnapshot = organizationPlanInfoOptionsSnapshot;
        }

        public async Task OnGetAsync()
        {
            OrganizationPlanInfos = _organizationPlanInfoOptionsSnapshot.Value;

            Organization = await _organizationAppService.GetProfileAsync(OrganizationName);

            if (CurrentUser.UserName != Organization.OwnerUserName)
            {
                throw new AbpAuthorizationException();
            }
        }

        public async Task<IActionResult> OnPostUpgradeAsync()
        {
            var organization = await GetOrganizationProfileAsync();

            var plan =  _organizationPlanInfoOptionsSnapshot.Value.FirstOrDefault(x => x.PlanType == TargetPlanToUpgrade && x.IsActive);
            if (plan is null)
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

            var plan =  _organizationPlanInfoOptionsSnapshot.Value.FirstOrDefault(x => x.PlanType == TargetPlanToUpgrade && x.IsActive);
            if (plan is null)
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
            OrganizationPlanInfoOptions plan,
            OrganizationProfileDto organization, 
            bool isExtend = false)
        {
            var paymentRequest = await _paymentRequestAppService.CreateAsync(new PaymentRequestCreationDto
            {
                CustomerId = CurrentUser.GetId().ToString(),
                Price = plan.Price,
                ProductId = OrganizationPlanInfoOptions.GetProductId(plan, isExtend),
                ProductName = OrganizationPlanInfoOptions.GetProductName(plan, organization.Name, isExtend),
                ExtraProperties =
                {
                    {
                        nameof(PaymentRequestProductExtraParameterConfiguration),
                        new PaymentRequestProductExtraParameterConfiguration
                        {
                            OrganizationName = OrganizationName,
                            TargetPlanType = TargetPlanToUpgrade,
                            IsExtend = isExtend
                        }
                    }
                }
            });
            
            return paymentRequest;
        }
    }
}