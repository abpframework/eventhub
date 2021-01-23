using System;
using Volo.Abp.DependencyInjection;

namespace EventHub
{
    public class EventHubTestData : ISingletonDependency
    {
        /* ORGANIZATIONS *****************************************************/

        public Guid OrganizationVolosoftId { get; internal set; }
        public string OrganizationVolosoftName { get; } = "volosoft";

        public Guid OrganizationDotnetEuropeId { get; internal set; }
        public string OrganizationDotnetEuropeName { get; } = "dotnet-europe";
    }
}
