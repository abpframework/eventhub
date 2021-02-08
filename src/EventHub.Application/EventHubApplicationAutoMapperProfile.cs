using AutoMapper;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using EventHub.Users;
using Volo.Abp.AutoMapper;

namespace EventHub
{
    public class EventHubApplicationAutoMapperProfile : Profile
    {
        public EventHubApplicationAutoMapperProfile()
        {
            CreateMap<Organization, OrganizationInListDto>();
            CreateMap<Organization, OrganizationProfileDto>();
            
            CreateMap<AppUser, OrganizationMemberDto>();

            CreateMap<Event, EventDto>();
            CreateMap<Event, EventInListDto>()
                .Ignore(x => x.OrganizationName)
                .Ignore(x => x.OrganizationDisplayName)
                .Ignore(x => x.IsLiveNow);
            CreateMap<Event, EventDetailDto>()
                .Ignore(x => x.OrganizationName)
                .Ignore(x => x.OrganizationDisplayName);

            CreateMap<AppUser, EventAttendeeDto>();

            CreateMap<Event, EventLocationDto>();
        }
    }
}
