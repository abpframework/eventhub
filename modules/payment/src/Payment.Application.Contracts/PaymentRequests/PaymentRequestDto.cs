using System;
using Volo.Abp.Application.Dtos;

namespace Payment.PaymentRequests
{
    public class PaymentRequestDto : CreationAuditedEntityDto<Guid>
    {
        public string CustomerId { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public PaymentRequestState State { get; set; }
    }
}