using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleAPI.Models
{
    public class Schedule : ISchedule
    {
        private readonly IDateTimeUtcNow _dateTimeUtcNow;
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

        public Schedule()
        {
            _dateTimeUtcNow = new DateTimeNow();
        }

        public Schedule(IDateTimeUtcNow dateTimeUtcNow)
        {
            _dateTimeUtcNow = dateTimeUtcNow;
        }

        private TimeZoneInfo TimeZoneInfo
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId); }
        }

        public DateTime? NextDeliveryDateTime
        {
            //optimized to only recalculate if private member is null
            get { return _nextDeliveryDateTime ?? (_nextDeliveryDateTime = CalculateNextDeliveryDateTimes(1).FirstOrDefault()); }
        }

        public IEnumerable<DateTime?> NextDeliveryDateTimes(int occurrences)
        {
            return CalculateNextDeliveryDateTimes(occurrences);
        }

        private IEnumerable<DateTime?> CalculateNextDeliveryDateTimes(int maxOccurences)
        {
            var retVal = new List<DateTime?>();
            DateTime utcNow = _dateTimeUtcNow.UtcNow;

            //can not calculate a next delivery date if start or activate dates do not exist.
            if (!StartDateTime.HasValue && !ActivateDateTime.HasValue)
            {
                return retVal;
            }

            //check if the stop date exists and has already passed.
            if (StopDateTime.HasValue && utcNow > StopDateTime.Value)
            {
                return retVal;
            }

            //check if this is the first delivery
            if (utcNow <= ActivateDateTime.Value)
            {
                retVal.Add(ActivateDateTime.Value);
                return retVal;
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
                        return CalculateHourlyNextDeliveryDateTimes(utcNow, maxOccurences);
                    case ScheduleRepeatTimeUnitCodes.Daily:
                        return CalculateDailyNextDeliveryDateTimes(utcNow, maxOccurences);
                    case ScheduleRepeatTimeUnitCodes.Weekly:
                        return CalculateWeeklyNextDeliveryDateTimes(utcNow, maxOccurences);
                    case ScheduleRepeatTimeUnitCodes.Monthly:
                        return CalculateMonthlyNextDeliveryDateTimes(utcNow, maxOccurences);
                }
            }

            //not a repeated delivery, and the single delivery date has already passed.
            return null;

        }

        private IEnumerable<DateTime?> CalculateMonthlyNextDeliveryDateTimes(DateTime fromUtcDateTime, int maxOccurences)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<DateTime?> CalculateWeeklyNextDeliveryDateTimes(DateTime fromUtcDateTime, int maxOccurences)
        {
            var retVal = new List<DateTime?>();
            var currDayOfWeek = fromUtcDateTime.DayOfWeek;
            var currDayOfWeekString = currDayOfWeek.ToString();
            var currRepeatDayOfWeek = (ScheduleRepeatDaysOfWeekFlags)Enum.Parse(typeof(ScheduleRepeatDaysOfWeekFlags), currDayOfWeekString);

            var dayNum = (int)currDayOfWeek;
            int addDays = 0;
            var occurences = 0;
            //find out how many days away we are from the next repeat day
            while (occurences < maxOccurences)
            {
                var nextUtcDateTime = fromUtcDateTime.AddDays(addDays);
                if (RepeatDaysOfWeek.HasFlag(currRepeatDayOfWeek) && nextUtcDateTime.Day > ActivateDateTime.Value.Day)
                {
                    addDays = (nextUtcDateTime - ActivateDateTime.Value).Days;
                    retVal.Add(ActivateDateTime.Value.AddDays(addDays));
                    occurences++;
                    if (occurences == maxOccurences)
                    {
                        //stop calculating delivery dates
                        continue;
                    }
                }

                addDays++;
                dayNum = (dayNum + 1) % 7;
                var nextDayOfWeek = (DayOfWeek)dayNum;
                var nextDayOfWeekString = nextDayOfWeek.ToString();
                currRepeatDayOfWeek = (ScheduleRepeatDaysOfWeekFlags)Enum.Parse(typeof(ScheduleRepeatDaysOfWeekFlags), nextDayOfWeekString);
            }

            return retVal;
        }

        private IEnumerable<DateTime?> CalculateDailyNextDeliveryDateTimes(DateTime fromUtcDateTime, int maxOccurences)
        {
            //TODO: add specific repeatcode logic here.
            throw new NotImplementedException();
        }

        private IEnumerable<DateTime?> CalculateHourlyNextDeliveryDateTimes(DateTime fromUtcDateTime, int maxOccurences)
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
