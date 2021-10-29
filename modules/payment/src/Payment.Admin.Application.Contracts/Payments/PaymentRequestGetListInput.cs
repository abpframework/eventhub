using System;
using Payment.PaymentRequests;
using Volo.Abp.Application.Dtos;

namespace Payment.Admin.Payments
{
    public class PaymentRequestGetListInput : PagedAndSortedResultRequestDto
    {
        public string ProductName { get; set; }

        public DateTime? MaxCreationTime { get; set; }

        public DateTime? MinCreationTime { get; set; }

        public PaymentRequestState? Status { get; set; }
    }
}