using System;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Events.Registrations;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Organizations.Plans;

public class PlanFeatureManager : ITransientDependency
{
    private readonly IPlanInfoDefinitionStore _planInfoDefinitionStore;
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IEventRegistrationRepository _eventRegistrationRepository;

    public PlanFeatureManager(
        IPlanInfoDefinitionStore planInfoDefinitionStore, 
        IRepository<Organization, Guid> organizationRepository, 
        IRepository<Event, Guid> eventRepository, 
        IEventRegistrationRepository eventRegistrationRepository)
    {
        _planInfoDefinitionStore = planInfoDefinitionStore;
        _organizationRepository = organizationRepository;
        _eventRepository = eventRepository;
        _eventRegistrationRepository = eventRegistrationRepository;
    }
    
    public async Task<bool> CanCreateNewEvent(Guid organizationId)
    {
        var currentPlanOfOrganization = await GetCurrentPlanByOrganizationIdAsync(organizationId);
        
        var totalEventCountByOrganization = await _eventRepository.CountAsync(x => x.OrganizationId == organizationId);
        totalEventCountByOrganization++;
        return currentPlanOfOrganization.Feature.MaxAllowedEventsCount >= totalEventCountByOrganization;
    }
    
    public async Task<bool> CanAddNewTrack(Event @event)
    {
        var currentPlanOfOrganization = await GetCurrentPlanByOrganizationIdAsync(@event.OrganizationId);

        var tracksCount = @event.Tracks.Count;
        tracksCount++;
        return currentPlanOfOrganization.Feature.MaxAllowedTracksCountInOneEvent >= tracksCount;
    }
    
    public async Task<bool> CanRegisterToEvent(Event @event)
    {
        var currentPlanOfOrganization = await GetCurrentPlanByOrganizationIdAsync(@event.OrganizationId);

        var attendeesCount = await _eventRegistrationRepository.CountAsync(x => x.EventId == @event.Id);
        attendeesCount++;
        return currentPlanOfOrganization.Feature.MaxAllowedAttendeesCountInOneEvent >= attendeesCount;
    }

    private async Task<PlanInfoDefinition> GetCurrentPlanByOrganizationIdAsync(Guid organizationId)
    {
        var organization = await _organizationRepository.GetAsync(organizationId);
        return await _planInfoDefinitionStore.GetPlanInfoByTypeAsync(organization.PlanType);
    }
}
