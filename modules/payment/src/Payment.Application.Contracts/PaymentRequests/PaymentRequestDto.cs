using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace Payment.PaymentRequests
{
    [Serializable]
    public class PaymentRequestDto : CreationAuditedEntityDto<Guid>, IHasExtraProperties
    {
        public string CustomerId { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public PaymentRequestState State { get; set; }

        public ExtraPropertyDictionary ExtraProperties { get; set; }

        public PaymentRequestDto()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}