using AutoMapper;
using EventHub.Admin.Events;
using EventHub.Admin.Events.Registrations;
using EventHub.Admin.Organizations;
using EventHub.Admin.Organizations.Memberships;
using EventHub.Admin.Users;
using EventHub.Countries;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;

namespace EventHub.Admin
{
    public class EventHubAdminApplicationAutoMapperProfile : Profile
    {
        public EventHubAdminApplicationAutoMapperProfile()
        {
            CreateMap<Organization, OrganizationInListDto>();

            CreateMap<Organization, OrganizationProfileDto>()
                .Ignore(x => x.OwnerUserName)
                .Ignore(x => x.OwnerEmail);

            CreateMap<OrganizationMemberWithDetails, OrganizationMemberDto>();

            CreateMap<Event, EventDetailDto>();

            CreateMap<Country, CountryLookupDto>();

            CreateMap<IdentityUser, EventAttendeeDto>()
                .ForMember(eventAttendee => eventAttendee.UserId,
                    opt => opt.MapFrom(user => user.Id));

            CreateMap<IdentityUser, UserDto>();

            CreateMap<EventWithDetails, EventInListDto>();

            CreateMap<EventRegistrationWithDetails, EventAttendeeDto>();
        }
    }
}
