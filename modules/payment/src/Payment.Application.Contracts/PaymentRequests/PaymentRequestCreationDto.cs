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
        
        public decimal Amount { get; set; }
    }
}