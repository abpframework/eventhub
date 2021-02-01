using System;
using Volo.Abp;

namespace EventHub.Events
{
    public static class EventUrlCodeHelper
    {
        public static string GetCodeFromUrl(string url)
        {
            Check.NotNullOrEmpty(url, nameof(url), minLength: EventConsts.UrlCodeLength);

            return url.Right(EventConsts.UrlCodeLength);
        }
    }
}
