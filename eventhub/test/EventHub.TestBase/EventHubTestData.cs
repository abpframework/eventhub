using System;
using Volo.Abp.DependencyInjection;

namespace EventHub
{
    public class EventHubTestData : ISingletonDependency
    {
        /* ORGANIZATIONS *****************************************************/

        public Guid OrganizationVolosoftId { get; } = Guid.NewGuid();
        public string OrganizationVolosoftName { get; } = "volosoft";

        public Guid OrganizationDotnetEuropeId { get; } = Guid.NewGuid();
        public string OrganizationDotnetEuropeName { get; } = "dotnet-europe";

        /* EVENTS ************************************************************/

        public Guid AbpBlazorPastEventId { get; } = Guid.NewGuid();
        public string AbpBlazorPastEventTitle { get; internal set; } = "ABP Framework Blazor UI Introduction";

        public Guid AbpMicroservicesFutureEventId { get;} = Guid.NewGuid();
        public string AbpMicroservicesFutureEventTitle { get; internal set; } = "ABP Framework Microservice Solution Development";
    }
}
