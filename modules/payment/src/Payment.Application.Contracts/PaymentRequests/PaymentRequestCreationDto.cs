using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace Payment.PaymentRequests
{
    [Serializable]
    public class PaymentRequestCreationDto : ExtensibleObject
    {
        [DynamicMaxLength(typeof(PaymentRequestConsts), nameof(PaymentRequestConsts.MaxCustomerIdLength))]
        public string CustomerId { get; set; }

        [DynamicMaxLength(typeof(PaymentRequestConsts), nameof(PaymentRequestConsts.MaxProductIdLength))]
        public string ProductId { get; set; }

        [Required]
        [DynamicMaxLength(typeof(PaymentRequestConsts), nameof(PaymentRequestConsts.MaxProductNameLength))]
        public string ProductName { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// 3 Letter Language Code in ISO 4217 Standards.
        /// For example: USD, EUR.
        /// </summary>
        [StringLength(PaymentRequestConsts.MaxCurrencyLength)]
        [CanBeNull]
        public string Currency { get; set; }
    }
}