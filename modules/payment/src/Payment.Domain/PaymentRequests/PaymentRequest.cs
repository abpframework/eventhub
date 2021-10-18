﻿using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Payment.PaymentRequests
{
    public class PaymentRequest : CreationAuditedAggregateRoot<Guid>, ISoftDelete
    {
        [CanBeNull]
        public string CustomerId { get; private set; }

        [CanBeNull]
        public string ProductId { get; private set; }

        [NotNull]
        public string ProductName { get; private set; }

        public decimal Price { get; private set; }

        public string Currency { get; private set; }

        public PaymentRequestState State { get; set; }

        public bool IsDeleted { get; set; }

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
            CustomerId = customerId;
            ProductId = productId;
            ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName));
            Price = price;
            Currency = currency;
        }
    }
}