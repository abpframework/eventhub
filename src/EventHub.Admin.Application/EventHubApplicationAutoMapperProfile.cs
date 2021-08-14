using AutoMapper;
using EventHub.Admin.Organizations;
using EventHub.Admin.Organizations.Memberships;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using Volo.Abp.AutoMapper;

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
        }
    }
}
