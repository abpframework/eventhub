using AutoMapper;
using EventHub.Events;
using EventHub.Organizations;
using EventHub.Web.Pages.Events.Components.CreateOrEditEventArea;
using EventHub.Web.Pages.Organizations;
using Volo.Abp.AutoMapper;

namespace EventHub.Web
{
    public class EventHubWebAutoMapperProfile : Profile
    {
        public EventHubWebAutoMapperProfile()
        {
            CreateMap<NewPageModel.CreateOrganizationViewModel, CreateOrganizationDto>();
            CreateMap<CreateOrEditEventAreaViewComponent.NewEventViewModel, CreateEventDto>()
                .Ignore(x => x.CoverImageStreamContent);
            CreateMap<OrganizationProfileDto, EditOrganizationViewModel>();
            CreateMap<EditOrganizationViewModel, UpdateOrganizationDto>();
            CreateMap<EventDetailDto, CreateOrEditEventAreaViewComponent.NewEventViewModel>();
        }
    }
}
