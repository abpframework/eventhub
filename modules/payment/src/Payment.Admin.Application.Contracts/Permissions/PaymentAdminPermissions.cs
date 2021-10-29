namespace Payment.Admin.Permissions
{
    public class PaymentAdminPermissions
    {
        public const string GroupName = "Payment";

        public static class Request
        {
            public const string Default = GroupName + ".Request";
        }
    }
}