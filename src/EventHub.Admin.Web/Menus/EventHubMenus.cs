namespace EventHub.Admin.Web.Menus
{
    public class EventHubMenus
    {
        public const string Prefix = "EventHub";

        public static class OrganizationManagement
        {
            public const string GroupName = Prefix + ".OrganizationManagement";
            public const string Organizations = GroupName + ".Organizations";
            public const string OrganizationMemberships = GroupName + ".OrganizationMemberships";
        }

        public static class EventManagement
        {
            public const string GroupName = Prefix + ".EventManagement";
            public const string Events = GroupName + ".Events";
        }
    }
}