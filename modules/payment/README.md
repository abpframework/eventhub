# Payment Module

Payment module provides an API to make payments via using PayPal easily.



## Installation

TODO



## Usage

1. Create a Payment Request via using `IPaymentRequestAppService`

   

2. Build an URL to redirect to checkout page via using `IPaymentUrlBuilder`

   ```csharp
   public class MyPageModel : AbpPageModel
   {
       protected IPaymentRequestAppService PaymentRequestAppService { get; }
       protected IPaymentUrlBuilder PaymentUrlBuilder { get; }
   
       public MyPageModel(
           IPaymentRequestAppService paymentRequestAppService,
           IPaymentUrlBuilder paymentUrlBuilder)
       {
           PaymentRequestAppService = paymentRequestAppService;
           PaymentUrlBuilder = paymentUrlBuilder;
       }
   
       public async Task OnPostAsync()
       {
           var paymentRequest = await PaymentRequestAppService.CreateAsync(new PaymentRequestCreationDto
           {
               Amount = 9.90m,
               CustomerId = CurrentUser.Id.ToString(),
               ProductId = "UniqueProductId",
               ProductName = "Awesome Product"
           });
   
           var checkoutUrl = PaymentUrlBuilder.BuildCheckoutUrl(paymentRequest.Id).AbsoluteUri;
   
           Response.Redirect(checkoutUrl);
       }
   }
   ```




---



## Distributed Events

- `Payment.Completed` (**PaymentRequestCompletedEto**): Published when a payment is completed. Completion can be triggered via webhook or callback. Source doesn't affect to the event. Event will be triggered once.
  - `PaymentRequestId`: Represents PaymentRequest entity Id.
  - `ExtraProperties`: Represents ExtraProperties of PaymentRequest entity. You can use this properties to find your related objects to that payment.
- `Payment.Failed`(**PaymentRequestFailedEto**): Published when a payment is failed. 
  - `PaymentRequestId`: Represents PaymentRequest entity Id.
  - `FailReason`: Reason of failure from payment provider (PayPal)
  - `ExtraProperties`: Represents ExtraProperties of PaymentRequest entity.

