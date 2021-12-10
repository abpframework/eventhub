using System;
using System.Globalization;
using System.Text;
using EventHub.Events;

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
        
        public static string GetTimeRangeText(TimeOnly startTime, TimeOnly endTime)
        {
            var sb = new StringBuilder();
            sb.Append(startTime.ToString("hh tt", CultureInfo.InvariantCulture));
            sb.Append(" - ");
            sb.Append(endTime.ToString("hh tt", CultureInfo.InvariantCulture));

            return sb.ToString();
        }
        
        public static string GetLocationInfo(EventInListDto eventItem)
        {
            if (eventItem.IsOnline)
            {
                return "online";
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(eventItem.City);
                sb.Append('/');
                sb.Append(eventItem.Country);
                return sb.ToString();
            }
        }
        
        public static string GetLocationInfo(bool isOnline, string city, string country, string separator)
        {
            if (isOnline)
            {
                return "online";
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(city);
                sb.Append(separator);
                sb.Append(country);
                return sb.ToString();
            }
        }
    }
}
