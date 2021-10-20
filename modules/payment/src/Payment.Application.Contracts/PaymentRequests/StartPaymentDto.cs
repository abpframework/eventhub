using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Payment.PaymentRequests
{
    [Serializable]
    public class StartPaymentDto
    {
        public Guid PaymentRequestId { get; set; }

        [NotNull]
        [Required]
        public string ReturnUrl { get; set; }

        public string CancelUrl { get; set; }
    }
}
