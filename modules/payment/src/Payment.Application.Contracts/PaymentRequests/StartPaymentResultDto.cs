using System;

namespace Payment.PaymentRequests
{
    [Serializable]
    public class StartPaymentResultDto
    {
        public string CheckoutLink { get; set; }
    }
}
