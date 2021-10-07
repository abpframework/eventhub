using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Payment.PaymentRequests
{
    public class PaymentRequest : CreationAuditedAggregateRoot<Guid>
    {
        public string CustomerId { get; private set; }
        
        public string ProductId { get; private set; }
        
        [NotNull]
        public string ProductName { get; private set; }
        
        public decimal Amount { get; private set; }

        public PaymentRequestState State { get; set; }

        public PaymentRequest(
            Guid id,
            [NotNull] string productName,
            [CanBeNull] string productId,
            decimal amount) 
            : base(id)
        {
            ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName));
            ProductId = productId;
            Amount = amount;
        }
    }
}