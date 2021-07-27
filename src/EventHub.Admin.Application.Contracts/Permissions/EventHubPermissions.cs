namespace EventHub.Admin.Permissions
{
    public static class EventHubPermissions
    {
        public const string GroupName = "EventHub";

        public static class Organizations
        {
            public const string Default = GroupName + ".Organizations";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
    }
}