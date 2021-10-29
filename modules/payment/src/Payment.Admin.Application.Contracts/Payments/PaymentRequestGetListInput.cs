using System;
using Payment.PaymentRequests;
using Volo.Abp.Application.Dtos;

namespace Payment.Admin.Payments
{
    public class PaymentRequestGetListInput : PagedAndSortedResultRequestDto
    {
        // public string CustomerId { get; set; }

        // public string ProductName { get; set; }

        public string Filter { get; set; }

        public DateTime? CreationDateMax { get; set; }

        public DateTime? CreationDateMin { get; set; }

        public PaymentRequestState? Status { get; set; }
    }
}