using System;
using Volo.Abp.DependencyInjection;

namespace EventHub
{
    public class EventHubSeedData : ISingletonDependency
    {
        //USERS
        public Guid UserSandraId { get; } = Guid.NewGuid();
        public Guid UserSergeyId { get; } = Guid.NewGuid();
        public Guid UserWellyId { get; } = Guid.NewGuid();
        public Guid UserAlessandroId { get; } = Guid.NewGuid();
        public Guid UserMarkId { get; } = Guid.NewGuid();
        public Guid UserTonyId { get; set; } = Guid.NewGuid();

        //ORGANIZATIONS
        public Guid OrganizationVolosoftId { get; } = Guid.NewGuid();
        public Guid OrganizationAbpId { get; } = Guid.NewGuid();
        public Guid OrganizationAngularCoderId { get; } = Guid.NewGuid();
        public Guid OrganizationDotnetWorldId { get; } = Guid.NewGuid();
        public Guid OrganizationDeveloperDaysId { get; } = Guid.NewGuid();
        public Guid OrganizationCSharpLoversId { get; } = Guid.NewGuid();
    }
}