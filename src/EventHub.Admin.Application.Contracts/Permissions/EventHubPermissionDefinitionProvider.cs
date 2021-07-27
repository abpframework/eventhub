using EventHub.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EventHub.Admin.Permissions
{
    public class EventHubPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var eventHubGroup = context.AddGroup(EventHubPermissions.GroupName);

            var tenantsPermission = eventHubGroup.AddPermission(EventHubPermissions.Organizations.Default, L("Permission:OrganizationManagement"));
            tenantsPermission.AddChild(EventHubPermissions.Organizations.Create, L("Permission:Create"));
            tenantsPermission.AddChild(EventHubPermissions.Organizations.Update, L("Permission:Edit"));
            tenantsPermission.AddChild(EventHubPermissions.Organizations.Delete, L("Permission:Delete"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<EventHubResource>(name);
        }
    }
}
