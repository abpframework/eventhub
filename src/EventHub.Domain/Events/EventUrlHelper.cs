using System;
using System.Text;

namespace EventHub.Events
{
    internal static class EventUrlHelper
    {
        private static string AllowedUrlChars = "abcdefghijklmnopqrstuvwxyz0123456789-";

        public static string ConvertTitleToUrlPart(string title)
        {
            var normalizedTitle = title
                .Trim()
                .Replace(' ', '-')
                .ToKebabCase()
                .ToLowerInvariant();

            var urlPartBuilder = new StringBuilder();

            char previousChar = ' ';
            foreach (var c in normalizedTitle)
            {
                if (AllowedUrlChars.Contains(c))
                {
                    if (previousChar == '-' && c == '-')
                    {
                        continue;
                    }
                    urlPartBuilder.Append(c);
                }

                if (urlPartBuilder.Length >= EventConsts.MaxTitleInUrlLength)
                {
                    break;
                }

                previousChar = c;
            }

            return urlPartBuilder.ToString();
        }
    }
}
