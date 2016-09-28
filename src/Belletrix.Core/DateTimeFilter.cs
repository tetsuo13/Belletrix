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

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Short, sweet, and simple.
        /// <see href="https://github.com/Humanizr/Humanizer">Humanizer</see>
        /// is another, more complete, method to accomplishing the same thing
        /// but will handle a lot more options.
        /// </remarks>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <seealso href="https://stackoverflow.com/a/1248">
        /// How do I calculate relative time?
        /// </seealso>
        public static string CalculateRelativeDate(DateTime date)
        {
            const int Second = 1;
            const int Minute = 60 * Second;
            const int Hour = 60 * Minute;
            const int Day = 24 * Hour;
            const int Month = 30 * Day;

            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - date.ToUniversalTime().Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * Minute)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * Minute)
            {
                return "a minute ago";
            }
            if (delta < 45 * Minute)
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 90 * Minute)
            {
                return "an hour ago";
            }
            if (delta < 24 * Hour)
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * Hour)
            {
                return "yesterday";
            }
            if (delta < 30 * Day)
            {
                return ts.Days + " days ago";
            }
            if (delta < 12 * Month)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}
