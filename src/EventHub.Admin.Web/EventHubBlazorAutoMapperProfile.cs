using AutoMapper;
using EventHub.Admin.Events;
using EventHub.Admin.Organizations;

namespace EventHub.Admin.Web
{
    public class EventHubBlazorAutoMapperProfile : Profile
    {
        public EventHubBlazorAutoMapperProfile()
        {
            CreateMap<OrganizationProfileDto, UpdateOrganizationDto>();

            CreateMap<EventDetailDto, UpdateEventDto>();
        }
    }
}