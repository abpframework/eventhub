using AutoMapper;
using EventHub.Events;
using EventHub.Organizations;
using Volo.Abp.AutoMapper;

namespace EventHub
{
    public class EventHubApplicationAutoMapperProfile : Profile
    {
        public EventHubApplicationAutoMapperProfile()
        {
            CreateMap<Organization, OrganizationInListDto>();
            CreateMap<Organization, OrganizationProfileDto>();

            CreateMap<Event, EventDto>();
            CreateMap<Event, EventInListDto>()
                .Ignore(x => x.OrganizationName)
                .Ignore(x => x.OrganizationDisplayName)
                .Ignore(x => x.IsLiveNow);
            CreateMap<Event, EventDetailDto>()
                .Ignore(x => x.OrganizationName)
                .Ignore(x => x.OrganizationDisplayName);
        }
    }
}
