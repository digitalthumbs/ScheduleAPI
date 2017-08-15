using System;

namespace ScheduleAPI.Models
{
    public interface ISchedule
    {
        Guid ScheduleId { get; set; }
        int CompanyId { get; set; }
        ScheduleItemTypes ScheduleItemType { get; set; }
        int ItemId { get; set; }

        string TimeZoneId { get; set; }
        DateTime? StartDateTime { get; set; }
        DateTime? StopDateTime { get; set; }
        DateTime? ActivateDateTime { get; set; }
        ScheduleRepeatCodes? RepeatCode { get; set; }
        ScheduleRepeatTimeUnitCodes? RepeatTimeUnitCode { get; set; }
        int? RepeatDayOfMonth { get; set; }
        ScheduleRepeatDaysOfWeekFlags RepeatDaysOfWeek { get; set; }

        //schedules should always calculate
        //the NextDeliveryDateTime and should never be assigned.
        DateTime? NextDeliveryDateTime { get; }
    }
}
