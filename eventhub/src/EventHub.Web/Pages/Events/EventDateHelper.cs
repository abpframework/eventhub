using System;
using System.Text;

namespace EventHub.Web.Pages.Events
{
    public static class EventDateHelper
    {
        public static string GetDateRangeText(DateTime startTime, DateTime endTime)
        {
            var sb = new StringBuilder();
            sb.Append(startTime.ToLongDateString());
            sb.Append(", ");
            sb.Append(startTime.ToString("hh:mm"));
            sb.Append(" - ");
            if (startTime.Day != endTime.Day)
            {
                sb.Append(endTime.ToLongDateString());
                sb.Append(", ");
            }

            sb.Append(endTime.ToString("hh:mm"));

            return sb.ToString();
        }
    }
}
