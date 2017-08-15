using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleAPI.Models
{
    public class Schedule : ISchedule
    {
        public Guid ScheduleId { get; set; }
        public int CompanyId { get; set; }
        public ScheduleItemTypes ScheduleItemType { get; set; }
        public int ItemId { get; set; }

        public string TimeZoneId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? StopDateTime { get; set; }
        public DateTime? ActivateDateTime { get; set; }
        public ScheduleRepeatCodes? RepeatCode { get; set; }
        public ScheduleRepeatTimeUnitCodes? RepeatTimeUnitCode { get; set; }
        public int? RepeatDayOfMonth { get; set; }
        public ScheduleRepeatDaysOfWeekFlags RepeatDaysOfWeek { get; set; }

        private DateTime? _nextDeliveryDateTime;


        private TimeZoneInfo TimeZoneInfo
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId); }
        }

        public DateTime? NextDeliveryDateTime
        {
            //optimized to only recalculate if private member is null
            get { return _nextDeliveryDateTime ?? (_nextDeliveryDateTime = CalculateNextDeliveryDate()); }
        }

        private DateTime? CalculateNextDeliveryDate()
        {
            DateTime utcNow = DateTime.UtcNow;

            //can not calculate a next delivery date if start or activate dates do not exist.
            if (!StartDateTime.HasValue && !ActivateDateTime.HasValue)
            {
                return null;
            }

            //check if the stop date exists and has already passed.
            if (StopDateTime.HasValue && utcNow > StopDateTime.Value)
            {
                return null;
            }

            //check if this is the first delivery
            if (utcNow <= ActivateDateTime.Value)
            {
                return ActivateDateTime.Value;
            }

            if (RepeatCode.GetValueOrDefault() == ScheduleRepeatCodes.Repeat)
            {
                //can not calculate nextDelivery if timeUnitcode is not set.
                if (!RepeatTimeUnitCode.HasValue)
                {
                    return null;
                }

                switch (RepeatTimeUnitCode.GetValueOrDefault())
                {
                    case ScheduleRepeatTimeUnitCodes.Hourly:
                        return CalculateHourlyNextDeliveryDateTime(utcNow);
                    case ScheduleRepeatTimeUnitCodes.Daily:
                        return CalculateDailyNextDeliveryDateTime(utcNow);
                    case ScheduleRepeatTimeUnitCodes.Weekly:
                        return CalculateWeeklyNextDeliveryDateTime(utcNow);
                    case ScheduleRepeatTimeUnitCodes.Monthly:
                        return CalculateMonthlyNextDeliveryDateTime(utcNow);
                }
            }

            //not a repeated delivery, and the single delivery date has already passed.
            return null;

        }

        private DateTime? CalculateMonthlyNextDeliveryDateTime(DateTime fromUtcDateTime)
        {
            throw new NotImplementedException();
        }

        private DateTime? CalculateWeeklyNextDeliveryDateTime(DateTime fromUtcDateTime)
        {
            var currDayOfWeek = fromUtcDateTime.DayOfWeek;
            var currDayOfWeekString = currDayOfWeek.ToString();
            var currRepeatDayOfWeek = (ScheduleRepeatDaysOfWeekFlags)Enum.Parse(typeof(ScheduleRepeatDaysOfWeekFlags), currDayOfWeekString);

            var dayNum = (int)currDayOfWeek;
            int addDays = 0;
            var found = false;
            //find out how many days away we are from the next repeat day
            while (!found)
            {
                var nextUtcDateTime = fromUtcDateTime.AddDays(addDays);
                if (RepeatDaysOfWeek.HasFlag(currRepeatDayOfWeek) && nextUtcDateTime.Day > ActivateDateTime.Value.Day)
                {
                    addDays = (nextUtcDateTime - ActivateDateTime.Value).Days;
                    found = true;
                    continue;
                }

                addDays++;
                dayNum = (dayNum + 1) % 7;
                var nextDayOfWeek = (DayOfWeek)dayNum;
                var nextDayOfWeekString = nextDayOfWeek.ToString();
                currRepeatDayOfWeek = (ScheduleRepeatDaysOfWeekFlags)Enum.Parse(typeof(ScheduleRepeatDaysOfWeekFlags), nextDayOfWeekString);
            }

            return ActivateDateTime.Value.AddDays(addDays);
        }

        private DateTime? CalculateDailyNextDeliveryDateTime(DateTime fromUtcDateTime)
        {
            //TODO: add specific repeatcode logic here.
            throw new NotImplementedException();
        }

        private DateTime? CalculateHourlyNextDeliveryDateTime(DateTime fromUtcDateTime)
        {
            //TODO: add specific repeatcode logic here.
            throw new NotImplementedException();
        }

        internal void ResetNextDeliveryDateTime()
        {
            _nextDeliveryDateTime = null;
        }
    }
}
