using System;
namespace JSONCapital.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static long UnixTimestampFromDateTime(this DateTime date)
        {
            long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }

        public static long UnixTimestampFromDateTime(this DateTime? date)
        {
            if (!date.HasValue) return UnixTimestampFromDateTime(default(DateTime));

            else return UnixTimestampFromDateTime(date);
        }

        public static DateTime DateTimeFromUnixTimestamp(this int unixTimestamp)
        {
            DateTime unixYear0 = new DateTime(1970, 1, 1);
            long unixTimeStampInTicks = unixTimestamp * TimeSpan.TicksPerSecond;
            DateTime dtUnix = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
            return dtUnix;
        }
    }
}
