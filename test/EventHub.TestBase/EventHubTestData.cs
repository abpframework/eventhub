using System;
using Volo.Abp.DependencyInjection;

namespace EventHub
{
    public class EventHubTestData : ISingletonDependency
    {
        /* USERS *************************************************************/

        public Guid UserAdminId { get; internal set; }
        public string UserAdminUserName { get; } = "admin";

        public Guid UserJohnId { get; } = Guid.NewGuid();
        public string UserJohnUserName { get; } = "john";

        /* ORGANIZATIONS *****************************************************/

        public Guid OrganizationVolosoftId { get; } = Guid.NewGuid();
        public string OrganizationVolosoftName { get; } = "volosoft";

        public Guid OrganizationDotnetEuropeId { get; } = Guid.NewGuid();
        public string OrganizationDotnetEuropeName { get; } = "dotnet-europe";

        /* EVENTS ************************************************************/

        public Guid AbpBlazorPastEventId { get; } = Guid.NewGuid();
        public string AbpBlazorPastEventTitle { get; } = "ABP Framework Blazor UI Introduction";
        public string AbpBlazorPastEventUrlCode { get; } = "00000001";

        public Guid AbpMicroservicesFutureEventId { get;} = Guid.NewGuid();
        public string AbpMicroservicesFutureEventTitle { get; } = "ABP Framework Microservice Solution Development";
        public string AbpMicroservicesFutureEventUrlCode { get; } = "00000002";
    }
}
