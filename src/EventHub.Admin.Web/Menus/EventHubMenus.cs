namespace EventHub.Admin.Web.Menus
{
    public class EventHubMenus
    {
        private const string Prefix = "EventHub";
        public const string Home = Prefix + ".Home";

        public static class OrganizationManagement
        {
            public const string GroupName = Prefix + ".OrganizationManagement";
            public const string Organizations = GroupName + ".Organizations";
            public const string OrganizationMemberships = GroupName + ".OrganizationMemberships";
        }
    }
}