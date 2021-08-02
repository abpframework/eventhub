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
        public const string EhAccount = "https://account.openeventhub.com";
        public const string EhApi = "https://api.openeventhub.com";
        public const string EhAdmin = "https://admin.openeventhub.com";
        public const string EhWww = "https://openeventhub.com";
#endif
    }
}