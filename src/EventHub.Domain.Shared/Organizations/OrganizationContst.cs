namespace EventHub.Organizations
{
    public static class OrganizationConsts
    {
        public const string DefaultSorting = "CreationTime desc";

        public const int MinNameLength = 2;
        public const int MaxNameLength = 32;

        public const int MinDisplayNameLength = 2;
        public const int MaxDisplayNameLength = 128;

        public const int MinDescriptionNameLength = 50;
        public const int MaxDescriptionNameLength = 1000;

        public const int MaxProfilePictureFileSize = 1 * 1024 * 1024;
        
        public const int MaxWebsiteLength = 512;
        
        public const int MaxTwitterUsernameLength = 24;
        
        public const int MaxGitHubUsernameLength = 24;

        public const int MaxFacebookUsernameLength = 24;

        public const int MaxInstagramUsernameLength = 24;

        public const int MaxMediumUsernameLength = 24;
        
        public const string ProductNamePrefix = "EventHub";

        public static string[] AllowedProfilePictureExtensions = { ".jpg", ".png" };
    }
}
