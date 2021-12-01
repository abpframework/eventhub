using System;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Payment.PaymentRequests
{
    public class PaymentRequest : CreationAuditedAggregateRoot<Guid>, ISoftDelete
    {
        [CanBeNull]
        public string CustomerId { get; protected set; }

        [CanBeNull]
        public string ProductId { get; protected set; }

        [NotNull]
        public string ProductName { get; protected set; }

        public decimal Price { get; protected set; }

        public string Currency { get; protected set; }

        public PaymentRequestState State { get; protected set; }

        public bool IsDeleted { get; set; }

        [CanBeNull]
        public string FailReason { get; protected set; }

        private PaymentRequest()
        {

        }

        public PaymentRequest(
            Guid id,
            [CanBeNull] string customerId,
            [CanBeNull] string productId,
            [NotNull] string productName,
            decimal price,
            [NotNull] string currency)
            : base(id)
        {
            CustomerId = Check.NotNullOrEmpty(customerId, nameof(customerId), maxLength: PaymentRequestConsts.MaxCustomerIdLength);
            ProductId = Check.NotNullOrEmpty(productId, nameof(productId), maxLength: PaymentRequestConsts.MaxProductIdLength);
            ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: PaymentRequestConsts.MaxProductNameLength);
            Price = price;
            Currency = currency;
        }

        public virtual void SetAsCompleted()
        {
            if (State == PaymentRequestState.Completed)
            {
                return;
            }

            State = PaymentRequestState.Completed;
            FailReason = null;

            AddDistributedEvent(new PaymentRequestCompletedEto
            {
                PaymentRequestId = Id,
                ExtraProperties = ExtraProperties
            });
        }

        public virtual void SetAsFailed(string failReason)
        {
            if (State == PaymentRequestState.Failed)
            {
                return;
            }

            State = PaymentRequestState.Failed;
            FailReason = failReason;

            AddDistributedEvent(new PaymentRequestFailedEto
            {
                PaymentRequestId = Id,
                FailReason = failReason,
                ExtraProperties = ExtraProperties
            });
        }
    }
}