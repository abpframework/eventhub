using EventHub.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EventHub.Permissions
{
    public class EventHubPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(EventHubPermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(EventHubPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<EventHubResource>(name);
        }
    }
}
