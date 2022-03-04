using System;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Events.Registrations;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;

namespace EventHub.Organizations.Plans;

public class PlanFeatureManager : ITransientDependency
{
    private readonly IOptions<PlanInfoOptions> _planInfoOptions;
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IEventRegistrationRepository _eventRegistrationRepository;
    private readonly IClock _clock;
    public PlanFeatureManager(
        IOptions<PlanInfoOptions> planInfoOptions, 
        IRepository<Organization, Guid> organizationRepository, 
        IRepository<Event, Guid> eventRepository, 
        IEventRegistrationRepository eventRegistrationRepository, 
        IClock clock)
    {
        _planInfoOptions = planInfoOptions;
        _organizationRepository = organizationRepository;
        _eventRepository = eventRepository;
        _eventRegistrationRepository = eventRegistrationRepository;
        _clock = clock;
    }
    
    public virtual async Task<bool> CanCreateNewEventAsync(Guid organizationId)
    {
        var currentPlanOfOrganization = await GetCurrentPlanByOrganizationIdAsync(organizationId);
        var totalEventCountByOrganizationInThisYear = await _eventRepository.CountAsync(x => x.OrganizationId == organizationId && x.CreationTime.Year == _clock.Now.Year);
        return currentPlanOfOrganization.Feature.MaxAllowedEventsCountInOneYear > totalEventCountByOrganizationInThisYear;
    }
    
    public virtual async Task<bool> CanAddNewTrackAsync(Event @event)
    {
        var currentPlanOfOrganization = await GetCurrentPlanByOrganizationIdAsync(@event.OrganizationId);

        var tracksCount = @event.Tracks.Count;
        return currentPlanOfOrganization.Feature.MaxAllowedTracksCountInOneEvent > tracksCount;
    }
    
    public virtual async Task<bool> CanRegisterToEventAsync(Event @event)
    {
        var currentPlanOfOrganization = await GetCurrentPlanByOrganizationIdAsync(@event.OrganizationId);

        var attendeesCount = await _eventRegistrationRepository.CountAsync(x => x.EventId == @event.Id);
        return currentPlanOfOrganization.Feature.MaxAllowedAttendeesCountInOneEvent > attendeesCount;
    }

    private async Task<PlanInfoDefinition> GetCurrentPlanByOrganizationIdAsync(Guid organizationId)
    {
        var organization = await _organizationRepository.GetAsync(organizationId);
        return _planInfoOptions.Value.GetPlanInfoByType(organization.PlanType);
    }
}
