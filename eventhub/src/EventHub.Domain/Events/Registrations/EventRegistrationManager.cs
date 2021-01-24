using System;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationManager : DomainService
    {
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;

        public EventRegistrationManager(
            IRepository<EventRegistration, Guid> eventRegistrationRepository
            )
        {
            _eventRegistrationRepository = eventRegistrationRepository;
        }
    }
}
