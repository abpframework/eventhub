using AutoMapper;
using EventHub.Events;
using EventHub.Organizations;
using EventHub.Web.Pages.Events.Components.CreateEventArea;
using EventHub.Web.Pages.Organizations;
using Volo.Abp.AutoMapper;
using EditPageModel = EventHub.Web.Pages.Events.EditPageModel;

namespace EventHub.Web
{
    public class EventHubWebAutoMapperProfile : Profile
    {
        public EventHubWebAutoMapperProfile()
        {
            CreateMap<NewPageModel.CreateOrganizationViewModel, CreateOrganizationDto>();
            CreateMap<CreateEventAreaViewComponent.NewEventViewModel, CreateEventDto>()
                .Ignore(x => x.CoverImageStreamContent);
            CreateMap<OrganizationProfileDto, EditOrganizationViewModel>();
            CreateMap<EditOrganizationViewModel, UpdateOrganizationDto>();
            CreateMap<EventDetailDto, CreateEventAreaViewComponent.NewEventViewModel>();
            CreateMap<EventDetailDto, EditPageModel.EditEventViewModel>();
            CreateMap<EditPageModel.EditEventViewModel, UpdateEventDto>();
        }
    }
}
