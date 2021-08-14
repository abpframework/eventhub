namespace EventHub.Web
{
    public class EventHubUrlOptions
    {
        private const string ConfigurationName = "AppUrls";

        public string Account { get; set; } = "https://localhost:44313";
        public string Www { get; set; } = "https://localhost:44308";
        public string Api { get; set; } = "https://localhost:44362";
        public string ApiInternal { get; set; } = "https://localhost:44362";
        public string Admin { get; set; } = "https://localhost:44307";
        public string AdminApi { get; set; } = "https://localhost:44305";

        public static string GetAccountConfigKey()
        {
            return GetConfigKey(nameof(Account));
        }
        
        public static string GetWwwConfigKey()
        {
            return GetConfigKey(nameof(Www));
        }
        
        public static string GetApiInternalConfigKey()
        {
            return GetConfigKey(nameof(ApiInternal));
        }
        
        public static string GetApiConfigKey()
        {
            return GetConfigKey(nameof(Api));
        }
        
        public static string GetAdminConfigKey()
        {
            return GetConfigKey(nameof(Admin));
        }
        
        public static string GetAdminApiConfigKey()
        {
            return GetConfigKey(nameof(AdminApi));
        }
        
        private static string GetConfigKey(string appName)
        {
            return $"{ConfigurationName}:{appName}";
        }
    }
}