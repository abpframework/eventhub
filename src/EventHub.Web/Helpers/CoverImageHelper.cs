using System;
using System.Linq;
using Volo.Abp;

namespace EventHub.Web.Helpers
{
    public class CoverImageHelper
    {
        private static readonly string[] ImageColors =
        {
            "linear-gradient(135deg, #41003d 0%, #a10097 100%)",
            "linear-gradient(135deg, #163e4b 0%, #297791 100%)",
            "linear-gradient(135deg, #660040 0%, #aa006b 100%)",
            "linear-gradient(135deg, #240d88 0%, #571fff 100%)",
        };

        public static string GetRandomColor(string articleTitle)
        {
            long total = articleTitle.Truncate(32).ToCharArray().Sum(c => c);
            return ImageColors[total % ImageColors.Length];
        }

        public static string GetTitle(string title)
        {
            title = Check.NotNullOrWhiteSpace(title, nameof(title));

            return title.ToUpper();
        }
    }
}