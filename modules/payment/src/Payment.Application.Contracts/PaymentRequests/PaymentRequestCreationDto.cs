using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;

namespace Payment.PaymentRequests
{
    public class PaymentRequestCreationDto : ExtensibleObject
    {
        public string CustomerId { get; set; }

        public string ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// 3 Letter Language Code in ISO 4217 Standards.
        /// For example: USD, EUR.
        /// </summary>
        [StringLength(3)]
        [CanBeNull]
        public string Currency { get; set; }
    }
}