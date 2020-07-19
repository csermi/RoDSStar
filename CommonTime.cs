using System;

namespace RoDSStar
{
   /// <summary>
    /// Common time constants and calculations.
    /// We use simple integer to measure time spent since StartTime (minutes) to calculate faster than with DateTime values.
    /// </summary>
    public static class CommonTime
    {
        public static DateTime StartTime = new DateTime(2020, 7, 20, 6, 0, 0, 0);

        public static readonly int MinutesInAHour = 60;
        public static int DayStartMinutes = 6 * MinutesInAHour;
        public static int DayEndMinutes = 22 * MinutesInAHour;
        public static readonly int TotalMinutesInADay = DayEndMinutes - DayStartMinutes;

        public static int ToMinutes(DateTime value)
        {
            var result = (int)(value - StartTime).TotalDays * TotalMinutesInADay;
            result += (GetTotalDayMinutes(value) - GetTotalDayMinutes(StartTime) + TotalMinutesInADay) % TotalMinutesInADay;

            return result;
        }

        public static DateTime ToDateTime(int value, bool isStartTime)
        {
            var day = value / TotalMinutesInADay;
            var minute = value - day * TotalMinutesInADay;
            DateTime result;
            if (isStartTime || (minute > 0) || (day == 0))
            {
                result = StartTime.AddDays(day).AddMinutes(minute);
            }
            else
            {
                // it is an ending time exactly at the end of day  
                result = StartTime.AddDays(day - 1).AddMinutes(TotalMinutesInADay);
            }
                   
            return result;
        }

        private static int GetTotalDayMinutes(DateTime value)
        {
            return value.Hour * MinutesInAHour + value.Minute;
        }

        public static int GetNumberOfDelayDays(int jobDueDate, int jobReady)
        {
            var diffMinutes = Math.Max(jobReady - jobDueDate, 0);
            var diffDays = (double) diffMinutes / TotalMinutesInADay;

            return (int)Math.Ceiling(diffDays);
        }
    }
}
