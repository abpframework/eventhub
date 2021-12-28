namespace EventHub.Events
{
    public static class EventConsts
    {
        public const string DefaultSorting = "Title desc";

        public const int UrlCodeLength = 8;
        public const int MaxTitleInUrlLength = 60;
        public const int MaxUrlLength = MaxTitleInUrlLength + 1 + UrlCodeLength; //Format: {Title}-{UrlCode}

        public const int MinTitleLength = 8;
        public const int MaxTitleLength = 128;

        public const int MinDescriptionLength = 50;
        public const int MaxDescriptionLength = 2000;

        public const int MinOnlineLinkLength = 4;
        public const int MaxOnlineLinkLength = 2000;

        public const int MinCityLength = 2;
        public const int MaxCityLength = 32;

        public const int MinLanguageLength = 2;
        public const int MaxLanguageLength = 16;

        public const int MaxTimingChangeCountForUser = 2;

        public const int MaxCoverImageFileSize = 5 * 1024 * 1024;

        public static string[] AllowedCoverImageExtensions = { ".jpg", ".png" };
    }
}
