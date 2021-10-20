using System;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Payment.PaymentRequests
{
    [Serializable]
    public class PaymentRequestCompletedEto : EtoBase, IHasExtraProperties
    {
        public Guid PaymentRequestId { get; set; }
        public ExtraPropertyDictionary ExtraProperties { get; set; }
    }
}
