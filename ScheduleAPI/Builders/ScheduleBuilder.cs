using ScheduleAPI.Models;
using ScheduleAPI.Utils;
using ScheduleAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleAPI.Builders
{
    public static class ScheduleBuilder
    {
        public static T BuildFromIScheduleForTimeZoneId<T>(ISchedule tzSchedule, string toFromTziId) where T : class, ISchedule, new()
        {
            if (tzSchedule == null)
            {
                throw new ArgumentNullException("tzSchedule");
            }

            if (typeof(T) == tzSchedule.GetType())
            {
                return (T)tzSchedule;
            }

            var toFromTzi = string.IsNullOrEmpty(toFromTziId) ? TimeZoneInfo.Utc : TimeZoneInfo.FindSystemTimeZoneById(toFromTziId);

            T iSchedule = new T()
            {
                ScheduleId = tzSchedule.ScheduleId,
                CompanyId = tzSchedule.CompanyId,
                ScheduleItemType = tzSchedule.ScheduleItemType,
                ItemId = tzSchedule.ItemId,
                RepeatCode = tzSchedule.RepeatCode,
                RepeatTimeUnitCode = tzSchedule.RepeatTimeUnitCode,
                RepeatDaysOfWeek = tzSchedule.RepeatDaysOfWeek,
                RepeatDayOfMonth = tzSchedule.RepeatDayOfMonth,
                TimeZoneId = toFromTzi.Id
            };

            if (toFromTzi == null)
            {
                //no conversion required
                iSchedule.ActivateDateTime = tzSchedule.ActivateDateTime;
                iSchedule.StartDateTime = tzSchedule.StartDateTime;
                iSchedule.StopDateTime = tzSchedule.StopDateTime;
                iSchedule.RepeatDaysOfWeek = tzSchedule.RepeatDaysOfWeek;
                iSchedule.RepeatDayOfMonth = tzSchedule.RepeatDayOfMonth;
                return iSchedule;
            }

            //conversion required
            // determine conversion direction
            Schedule s;
            ConversionDirections utcCd;
            if (tzSchedule is Schedule)
            {
                s = (Schedule)tzSchedule;
                utcCd = ConversionDirections.From;
            }
            else
            {
                s = iSchedule as Schedule;
                utcCd = ConversionDirections.To;
            }

            //convert 
            var hasActiveDateTime = tzSchedule.ActivateDateTime.HasValue;
            iSchedule.ActivateDateTime = hasActiveDateTime
                ? ConvertDateTimeToFromUtcForTimeZone(tzSchedule.ActivateDateTime.Value, utcCd, toFromTzi)
                : tzSchedule.ActivateDateTime;

            iSchedule.StartDateTime = tzSchedule.StartDateTime.HasValue
                ? ConvertDateTimeToFromUtcForTimeZone(tzSchedule.StartDateTime.Value, utcCd, toFromTzi)
                : tzSchedule.StartDateTime;

            iSchedule.StopDateTime = tzSchedule.StopDateTime.HasValue
                ? ConvertDateTimeToFromUtcForTimeZone(tzSchedule.StopDateTime.Value, utcCd, toFromTzi)
                : tzSchedule.StopDateTime;


            if (hasActiveDateTime && tzSchedule.RepeatCode == ScheduleRepeatCodes.Repeat)
            {
                switch (tzSchedule.RepeatTimeUnitCode)
                {
                    case ScheduleRepeatTimeUnitCodes.Weekly:
                        iSchedule.RepeatDaysOfWeek = ConversionUtil.ConvertRepeatDaysOfWeekForTimeZone(s, utcCd, toFromTzi);
                        break;
                    case ScheduleRepeatTimeUnitCodes.Monthly:
                        iSchedule.RepeatDayOfMonth = tzSchedule.RepeatDayOfMonth; // TODO: Execute Conversion based on source timeZone.
                        break;
                }
            }

            SetNextDeliveryDateTime(iSchedule, tzSchedule, toFromTzi);

            return iSchedule;
        }

        private static void SetNextDeliveryDateTime(ISchedule iSchedule, ISchedule tzSchedule, TimeZoneInfo tzi)
        {
            //only set the NextDeliveryDateTime on a ScheduleViewModel, schedules should always calculate
            //the NextDeliveryDateTime and should never be assigned.
            if (iSchedule is ScheduleViewModel)
            {
                var s = iSchedule as ScheduleViewModel;
                s.NextDeliveryDateTime = tzSchedule.NextDeliveryDateTime.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(tzSchedule.NextDeliveryDateTime.Value, tzi)
                    : tzSchedule.NextDeliveryDateTime;
            }
        }

        private static DateTime? ConvertDateTimeToFromUtcForTimeZone(DateTime dt, ConversionDirections cd, TimeZoneInfo tzi)
        {
            if (tzi == TimeZoneInfo.Utc)
            {
                return dt;
            }

            switch (cd)
            {
                case ConversionDirections.To:
                    return TimeZoneInfo.ConvertTimeToUtc(dt, tzi);
                case ConversionDirections.From:
                    return TimeZoneInfo.ConvertTimeFromUtc(dt, tzi);
                default:
                    return null;
            }
        }
    }
}
