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

            var organizationsPermission = eventHubGroup.AddPermission(EventHubPermissions.Organizations.Default, L("Permission:OrganizationManagement"));
            organizationsPermission.AddChild(EventHubPermissions.Organizations.Create, L("Permission:Create"));
            organizationsPermission.AddChild(EventHubPermissions.Organizations.Update, L("Permission:Edit"));
            organizationsPermission.AddChild(EventHubPermissions.Organizations.Delete, L("Permission:Delete"));
            organizationsPermission.AddChild(EventHubPermissions.Organizations.Memberships.Default, L("Permission:MembershipManagement"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<EventHubResource>(name);
        }
    }
}
