using AutoMapper;
using EventHub.Admin.Organizations;
using EventHub.Admin.Organizations.Memberships;
using EventHub.Organizations;
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

            CreateMap<IdentityUser, OrganizationMemberDto>();
        }
    }
}
