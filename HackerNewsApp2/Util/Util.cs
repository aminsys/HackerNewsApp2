using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsApp2.Util
{
    public class Util
    {
        private static readonly long SECOND_MILLIS = 1000;
        private static readonly long MINUTE_MILLIS = 60 * SECOND_MILLIS;
        private static readonly long HOUR_MILLIS = 60 * MINUTE_MILLIS;
        private static readonly long DAY_MILLIS = 24 * HOUR_MILLIS;
        private static readonly long YEAR_MILLIS = 365 * DAY_MILLIS;

        public static string GetTimeAgo(DateTime createdAt)
        {
            long time = (long)(createdAt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            if (time < 1000000000000L)
            {
                // if timestamp given in seconds, convert to millis
                time *= 1000;
            }

            long now = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            if (time > now || time <= 0)
            {
                return "?";
            }

            long diff = now - time;
            if (diff < MINUTE_MILLIS)
            {
                return "just now";
            }
            else if (diff < 2 * MINUTE_MILLIS)
            {
                return "1 min";
            }
            else if (diff < 50 * MINUTE_MILLIS)
            {
                return diff / MINUTE_MILLIS + " mins";
            }
            else if (diff < 120 * MINUTE_MILLIS)
            {
                return "1 hr";
            }
            else if (diff < 24 * HOUR_MILLIS)
            {
                return diff / HOUR_MILLIS + " hrs";
            }
            else if (diff < 48 * HOUR_MILLIS)
            {
                return "1 day";
            }
            else if (diff < 365 * DAY_MILLIS)
            {
                return diff / DAY_MILLIS + " days";
            }
            else if (diff < 2 * YEAR_MILLIS)
            {
                return "1 year";
            }
            else
            {
                return diff / YEAR_MILLIS + " years";
            }
        }
    }
}
