using System;

namespace Payment.Web.PaymentRequest
{
    public interface IPaymentUrlBuilder
    {
        Uri BuildCheckoutUrl(Guid paymentRequestId);
    }
}