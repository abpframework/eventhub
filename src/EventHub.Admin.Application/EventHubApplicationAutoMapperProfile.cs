using AutoMapper;
using EventHub.Admin.Events;
using EventHub.Admin.Events.Registrations;
using EventHub.Admin.Organizations;
using EventHub.Admin.Organizations.Memberships;
using EventHub.Admin.Users;
using EventHub.Countries;
using EventHub.Events;
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
                .Ignore(x => x.OwnerEmail)
                .Ignore(x => x.ProfilePictureContent);

            CreateMap<OrganizationMemberWithDetails, OrganizationMemberDto>();

            CreateMap<Event, EventDetailDto>()
                .Ignore(x => x.CoverImageContent);

            CreateMap<Country, CountryLookupDto>();

            CreateMap<IdentityUser, EventAttendeeDto>();
            CreateMap<IdentityUser, UserDto>();
        }
    }
}
