using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace EventHub
{
    public class EventHubTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly OrganizationManager _organizationManager;
        private readonly ICurrentUser _currentUser;
        private readonly EventHubTestData _eventHubTestData;

        public EventHubTestDataSeedContributor(
            EventHubTestData eventHubTestData,
            OrganizationManager organizationManager,
            ICurrentUser currentUser)
        {
            _eventHubTestData = eventHubTestData;
            _organizationManager = organizationManager;
            _currentUser = currentUser;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await CreateOrganizationsAsync();
        }

        private async Task CreateOrganizationsAsync()
        {
            var volosoft = await _organizationManager.CreateAsync(
                _currentUser.GetId(),
                _eventHubTestData.OrganizationVolosoftName,
                "Volosoft",
                "Volosoft is producing software development tools for developers. We are organizing events related to the ABP.IO platform and general software development topic."
            );
            _eventHubTestData.OrganizationVolosoftId = volosoft.Id;

            var dotnetEurope = await _organizationManager.CreateAsync(
                _currentUser.GetId(),
                _eventHubTestData.OrganizationDotnetEuropeName,
                "Dotnet Europe",
                "Organizing events on Microsoft's .NET Platform in European Countries."
            );
            _eventHubTestData.OrganizationDotnetEuropeId = dotnetEurope.Id;
        }
    }
}
