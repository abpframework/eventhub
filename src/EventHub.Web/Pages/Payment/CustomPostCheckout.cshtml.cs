using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payment.PaymentRequests;
using Payment.Web;
using Payment.Web.Pages.Payment;
using Payment.Web.PaymentRequest;

namespace EventHub.Web.Pages.Payment;

public class CustomPostCheckoutPageModel : PostCheckoutPageModel
{
    public string OrganizationName { get; set; }
    
    public CustomPostCheckoutPageModel(
        IPaymentRequestAppService appService, 
        IOptions<PaymentWebOptions> paymentWebOptions, 
        IPaymentUrlBuilder paymentUrlBuilder) 
        : base(
            appService, 
            paymentWebOptions, 
            paymentUrlBuilder)
    {
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (Token.IsNullOrWhiteSpace())
        {
            return BadRequest();
        }

        PaymentRequest = await PaymentRequestAppService.CompleteAsync(Token);
        
        FillOrganizationName();
        PaymentWebOptions.PaymentSuccessfulCallbackUrl = Url.Page("/Organizations/Profile", new { name = OrganizationName });

        GoBackLink = PaymentUrlBuilder.BuildCheckoutUrl(PaymentRequest.Id).AbsoluteUri;

        if (PaymentRequest.State == PaymentRequestState.Completed 
            && !PaymentWebOptions.PaymentSuccessfulCallbackUrl.IsNullOrWhiteSpace())
        {
            var callbackUrl = PaymentWebOptions.PaymentSuccessfulCallbackUrl + "?paymentRequestId=" + PaymentRequest.Id;
            Response.Redirect(callbackUrl);
        }

        if (PaymentRequest.State == PaymentRequestState.Failed
            && !PaymentWebOptions.PaymentFailureCallbackUrl.IsNullOrWhiteSpace())
        {
            var callbackUrl = PaymentWebOptions.PaymentFailureCallbackUrl + "?paymentRequestId=" + PaymentRequest.Id;
            Response.Redirect(callbackUrl);
        }

        return Page();
    }

    private void FillOrganizationName()
    {
        var isExistExtraProperties = PaymentRequest.ExtraProperties.TryGetValue(nameof(OrganizationPaymentRequestExtraParameterConfiguration), out var ExtraProperties);
        if (!isExistExtraProperties)
        {
            throw new BadHttpRequestException("");
        }
    
        var organizationPaymentExtraParameterConfiguration = JsonConvert.DeserializeObject<OrganizationPaymentRequestExtraParameterConfiguration>(ExtraProperties.ToString());
        if (organizationPaymentExtraParameterConfiguration is null)
        {
            throw new BadHttpRequestException("");
        }

        OrganizationName = organizationPaymentExtraParameterConfiguration.OrganizationName;
    }
}
