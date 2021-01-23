using AutoMapper;
using EventHub.Organizations;
using EventHub.Web.Pages.Organizations;

namespace EventHub.Web
{
    public class EventHubWebAutoMapperProfile : Profile
    {
        public EventHubWebAutoMapperProfile()
        {
            CreateMap<New.CreateOrganizationViewModel, CreateOrganizationDto>();
        }
    }
}
