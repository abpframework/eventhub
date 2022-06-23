using Microsoft.Extensions.Configuration;

namespace EventHub.Options
{
    public class EventHubUrlOptions
    {
        private const string ConfigurationName = "AppUrls";
        private const string AccountDefaultValue = "https://localhost:44313";
        private const string AdminDefaultValue = "https://localhost:44307";
        private const string WwwDefaultValue = "https://localhost:44308";
        private const string ApiDefaultValue = "https://localhost:44362";
        private const string ApiInternalDefaultValue = ApiDefaultValue;
        private const string AdminApiDefaultValue = "https://localhost:44305";

        public string Account { get; set; } = AccountDefaultValue;
        public string Www { get; set; } = WwwDefaultValue;
        public string Api { get; set; } = ApiDefaultValue;
        public string ApiInternal { get; set; } = ApiInternalDefaultValue;
        public string Admin { get; set; } = AdminDefaultValue;
        public string AdminApi { get; set; } = AdminApiDefaultValue;

        public static string GetAccountConfigValue(IConfiguration configuration)
        {
            return GetConfigValue(configuration, nameof(Account), AccountDefaultValue);
        }

        public static string GetWwwConfigValue(IConfiguration configuration)
        {
            return GetConfigValue(configuration, nameof(Www), WwwDefaultValue);
        }

        public static string GetApiInternalConfigValue(IConfiguration configuration)
        {
            return GetConfigValue(configuration, nameof(ApiInternal), ApiInternalDefaultValue);
        }

        public static string GetApiConfigValue(IConfiguration configuration)
        {
            return GetConfigValue(configuration, nameof(Api), ApiDefaultValue);
        }

        public static string GetAdminConfigValue(IConfiguration configuration)
        {
            return GetConfigValue(configuration, nameof(Admin), AdminDefaultValue);
        }

        public static string GetAdminApiConfigValue(IConfiguration configuration)
        {
            return GetConfigValue(configuration, nameof(AdminApi), AdminApiDefaultValue);
        }

        private static string GetConfigKey(string appName)
        {
            return $"{ConfigurationName}:{appName}";
        }

        private static string GetConfigValue(
            IConfiguration configuration,
            string appName,
            string defaultValue)
        {
            return configuration[GetConfigKey(appName)] ?? defaultValue;
        }
    }
}
