using AutoMapper;
using EventHub.Events;
using EventHub.Organizations;

namespace EventHub.Web
{
    public class EventHubWebAutoMapperProfile : Profile
    {
        public EventHubWebAutoMapperProfile()
        {
            CreateMap<EventHub.Web.Pages.Organizations.NewPageModel.CreateOrganizationViewModel, CreateOrganizationDto>();
            CreateMap<EventHub.Web.Pages.Events.NewPageModel.NewEventViewModel, CreateEventDto>();
        }
    }
}
