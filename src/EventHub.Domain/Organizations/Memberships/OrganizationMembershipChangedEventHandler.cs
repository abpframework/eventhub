using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;

namespace EventHub.Domain.Organizations.Memberships
{
	public class OrganizationMembershipChangedEventHandler : ILocalEventHandler<EntityCreatedEventData<OrganizationMembership>>,
		 ILocalEventHandler<EntityDeletedEventData<OrganizationMembership>>, ITransientDependency
	{
		private readonly IRepository<Organization, Guid> _organizationRepository;

		public OrganizationMembershipChangedEventHandler(IRepository<Organization, Guid> organizationRepository)
		{
			_organizationRepository = organizationRepository;
		}

		public async Task HandleEventAsync(EntityCreatedEventData<OrganizationMembership> eventData)
		{
			var organization = await _organizationRepository.GetAsync(eventData.Entity.OrganizationId);

			organization.MemberCount++;

			await _organizationRepository.UpdateAsync(organization);
		}

		public async Task HandleEventAsync(EntityDeletedEventData<OrganizationMembership> eventData)
		{
			var organization = await _organizationRepository.GetAsync(eventData.Entity.OrganizationId);

			organization.MemberCount--;

			await _organizationRepository.UpdateAsync(organization);
		}
	}
}
