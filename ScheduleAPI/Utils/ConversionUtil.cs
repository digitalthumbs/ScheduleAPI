using ScheduleAPI.Extensions;
using ScheduleAPI.Models;
using System;

namespace ScheduleAPI.Utils
{
    public static class ConversionUtil
    {
        internal static ScheduleRepeatDaysOfWeekFlags ConvertRepeatDaysOfWeekForTimeZone(Schedule schedule, ConversionDirections toFromUtc, TimeZoneInfo tzi)
        {
            if (!schedule.ActivateDateTime.HasValue)
            {
                throw new ArgumentNullException("schdeule", "The schedule passed does not have a value for the ActivateDateTime property");
            }

            var retValue = schedule.RepeatDaysOfWeek;

            var convertedActivateDateTime = TimeZoneInfo.ConvertTimeFromUtc(schedule.ActivateDateTime.Value, tzi);

            if (convertedActivateDateTime.Day != schedule.ActivateDateTime.Value.Day)
            {
                var original = (int)schedule.RepeatDaysOfWeek;
                TimeSpan ts = toFromUtc == ConversionDirections.To
                    ? convertedActivateDateTime - schedule.ActivateDateTime.Value
                    : schedule.ActivateDateTime.Value - convertedActivateDateTime;

                //always shift by a minimum of 1 day
                var shiftDaysNum = Convert.ToByte(Math.Abs(Math.Round(ts.TotalDays)) + 1);
                int convertedRepeatDaysOfWeekFlagsValue = 0;
                if (ts.TotalDays > 0)
                {
                    // rotate the day flags to the right by x bit(s)
                    convertedRepeatDaysOfWeekFlagsValue = original.RotateRight(7, shiftDaysNum);
                }

                if (ts.TotalDays < 0)
                {
                    // rotate the day flags to the left by x bit(s)
                    convertedRepeatDaysOfWeekFlagsValue = original.RotateLeft(7, shiftDaysNum);
                }

                retValue = (ScheduleRepeatDaysOfWeekFlags)convertedRepeatDaysOfWeekFlagsValue;
            }

            return retValue;
        }
    }
}
