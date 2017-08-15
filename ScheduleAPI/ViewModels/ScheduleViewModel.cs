using ScheduleAPI.Models;
using System;


namespace ScheduleAPI.ViewModels
{
    public class ScheduleViewModel : ISchedule
    {
        public int CompanyId { get; set; }
        public ScheduleItemTypes ScheduleItemType { get; set; }
        public int ItemId { get; set; }

        public DateTime? StartDateTime { get; set; }
        public DateTime? StopDateTime { get; set; }
        public DateTime? ActivateDateTime { get; set; }
        public ScheduleRepeatCodes? RepeatCode { get; set; }
        public ScheduleRepeatTimeUnitCodes? RepeatTimeUnitCode { get; set; }
        public int? RepeatDayOfMonth { get; set; }
        public ScheduleRepeatDaysOfWeekFlags RepeatDaysOfWeek { get; set; }
        public DateTime? NextDeliveryDateTime { get; internal set; }
        public Guid ScheduleId { get; set; }
        public string TimeZoneId { get; set; }
    }
}
