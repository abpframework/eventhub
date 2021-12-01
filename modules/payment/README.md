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



## Webhook Handling
You have to make some configuration for webhooks on PayPal Dashboard.

_If you test it locally, you can use [ngrok](https://ngrok.com/) to open your localhost to world_

0. Deploy eventhub or open your ip:port to public. Your HttpApi.Host must be accessible from all over the world.

1. Go to [Application List](https://developer.paypal.com/developer/applications). Find your app and go details. _(You can use my account below)_
<details>
  <summary>PayPal Account Credentials</summary>

> `info@enisnecipoglu.com`
> `1q2w3E**`

> Use following temp number for two factor authentication verification:
> https://receive-smss.com/sms/48722717428/

> _(Yes the star (*) is doubled. min password length requirement was 8)_

</details> 

2. At the bottom of page you'll see the Webhooks section
![image](https://user-images.githubusercontent.com/23705418/138262017-b2c3a689-f929-4dbb-8927-04ab9a2c5292.png)

3. Click to **Add Webhook** button and use following values:
    - **Webhook URL**: `https://yourhttpapihosturl.com/api/payment/requests/webhook` _(replace hostname with your API public url)_
    - **Event types**: `Checkout order approved`, `Checkout order completed`
