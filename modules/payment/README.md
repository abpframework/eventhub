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

   

   