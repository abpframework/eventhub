using AutoMapper;
using EventHub.Countries;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Members;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using EventHub.Users;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;

namespace EventHub
{
    public class EventHubApplicationAutoMapperProfile : Profile
    {
        public EventHubApplicationAutoMapperProfile()
        {
            CreateMap<Organization, OrganizationInListDto>();
            CreateMap<Organization, OrganizationProfileDto>();
            CreateMap<Organization, OrganizationDto>();

            CreateMap<IdentityUser, OrganizationMemberDto>();

            CreateMap<Event, EventDto>();
            CreateMap<Event, EventInListDto>()
                .Ignore(x => x.OrganizationName)
                .Ignore(x => x.OrganizationDisplayName)
                .Ignore(x => x.IsLiveNow);
            CreateMap<Event, EventDetailDto>()
                .ForMember(x => x.Tracks, memberOptions => memberOptions.MapFrom(m => m.Tracks))
                .Ignore(x => x.OrganizationId)
                .Ignore(x => x.OrganizationName)
                .Ignore(x => x.OrganizationDisplayName);

            CreateMap<Track, TrackDto>()
                .ForMember(x => x.Sessions, memberOptions => memberOptions.MapFrom(m => m.Sessions));

            CreateMap<Session, SessionDto>()
                .ForMember(x => x.Speakers, memberOptions => memberOptions.MapFrom(m => m.Speakers));
                
            CreateMap<Speaker, SpeakerDto>()
                .Ignore(x => x.UserName);

            CreateMap<IdentityUser, EventAttendeeDto>();

            CreateMap<Event, EventLocationDto>()
                .Ignore(x => x.Country);
            
            CreateMap<Country, CountryLookupDto>();

            CreateMap<IdentityUser, UserDto>();
        }
    }
}
