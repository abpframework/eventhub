using AutoMapper;
using EventHub.Admin.Organizations;
using EventHub.Organizations;
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
        }
    }
}
