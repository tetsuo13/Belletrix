using System;

namespace Belletrix.Core
{
    public static class DateTimeFilter
    {
        public static DateTime UtcToLocal(DateTime date)
        {
            try
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(date, tz);
            }
            catch (Exception)
            {
            }

            return date;
        }
    }
}
