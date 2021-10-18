using System;

namespace Payment.Web.PaymentRequest
{
    public interface IPaymentUrlBuilder
    {
        Uri BuildCheckoutUrl(Guid paymentRequestId);

        Uri BuildReturnUrl(Guid paymentRequestId);
    }
}