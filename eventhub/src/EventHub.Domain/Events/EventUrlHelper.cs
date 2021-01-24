using System;
using System.Text;

namespace EventHub.Events
{
    internal static class EventUrlHelper
    {
        private static string AllowedUrlChars = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static string ConvertTitleToUrlPart(string title)
        {
            var normalizedTitle = title
                .Replace(' ', '-')
                .ToKebabCase()
                .ToLowerInvariant();

            var urlPartBuilder = new StringBuilder();

            foreach (var c in normalizedTitle)
            {
                if (AllowedUrlChars.Contains(c))
                {
                    urlPartBuilder.Append(c);
                }

                if (urlPartBuilder.Length >= EventConsts.MaxTitleInUrlLength)
                {
                    break;
                }
            }

            return urlPartBuilder.ToString();
        }
    }
}
