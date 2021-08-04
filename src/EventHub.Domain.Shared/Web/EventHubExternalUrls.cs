namespace EventHub.Web
{
    public static class EventHubExternalUrls
    {
#if DEBUG
        public const string EhAccount = "https://localhost:44313";
        public const string EhApi = "https://localhost:44362";
        public const string EhAdmin = "https://localhost:44307";
        public const string EhWww = "https://localhost:44308";
#else
        // TODO: Change these production links
        public const string EhAccount = "https://eh-st-account";
        public const string EhApi = "https://eh-st-api";
        public const string EhAdmin = "https://eh-st-admin";
        public const string EhWww = "https://eh-st-www";
#endif
    }
}