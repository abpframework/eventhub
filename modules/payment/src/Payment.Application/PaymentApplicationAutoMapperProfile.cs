using AutoMapper;
using Payment.PaymentRequests;

namespace Payment
{
    public class PaymentApplicationAutoMapperProfile : Profile
    {
        public PaymentApplicationAutoMapperProfile()
        {
            CreateMap<PaymentRequest, PaymentRequestDto>();
        }
    }
}