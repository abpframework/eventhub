using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace EventHub
{
    public class EventHubTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly OrganizationManager _organizationManager;
        private readonly EventHubTestData _eventHubTestData;

        public EventHubTestDataSeedContributor(
            IRepository<Organization, Guid> organizationRepository,
            EventHubTestData eventHubTestData,
            OrganizationManager organizationManager)
        {
            _organizationRepository = organizationRepository;
            _eventHubTestData = eventHubTestData;
            _organizationManager = organizationManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await CreateOrganizationsAsync();
        }

        private async Task CreateOrganizationsAsync()
        {
            var volosoft = await _organizationManager.CreateAsync(
                Guid.NewGuid(),
                _eventHubTestData.OrganizationVolosoftName,
                "Volosoft",
                "Volosoft is producing software development tools for developers. We are organizing events related to the ABP.IO platform and general software development topic."
            );
            _eventHubTestData.OrganizationVolosoftId = volosoft.Id;

            var dotnetEurope = await _organizationManager.CreateAsync(
                Guid.NewGuid(),
                _eventHubTestData.OrganizationDotnetEuropeName,
                "Dotnet Europe",
                "Organizing events on Microsoft's .NET Platform in European Countries."
            );
            _eventHubTestData.OrganizationDotnetEuropeId = dotnetEurope.Id;
        }
    }
}
