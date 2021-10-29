using AutoMapper;
using Payment.Admin.Payments;
using Payment.PaymentRequests;

namespace Payment.Admin
{
    public class PaymentAdminApplicationAutoMapperProfile : Profile
    {
        public PaymentAdminApplicationAutoMapperProfile()
        {
            CreateMap<PaymentRequest, PaymentRequestWithDetailsDto>();
        }
    }
}