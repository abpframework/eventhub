using AutoMapper;
using EventHub.Events;
using EventHub.Organizations;

namespace EventHub
{
    public class EventHubApplicationAutoMapperProfile : Profile
    {
        public EventHubApplicationAutoMapperProfile()
        {
            CreateMap<Organization, OrganizationInListDto>();
            CreateMap<Organization, OrganizationProfileDto>();

            CreateMap<Event, EventDto>();
        }
    }
}
