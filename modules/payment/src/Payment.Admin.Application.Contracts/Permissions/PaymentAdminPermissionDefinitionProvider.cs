using Payment.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Payment.Admin.Permissions
{
    public class PaymentAdminPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var paymentGroup = context.AddGroup(PaymentAdminPermissions.GroupName, L("Permission:Payment"));
            paymentGroup.AddPermission(PaymentAdminPermissions.Request.Default, L("Permission:RequestManagement"));
        }
        
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<PaymentResource>(name);
        }
    }
}