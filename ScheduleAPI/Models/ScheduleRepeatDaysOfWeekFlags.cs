using System;

namespace ScheduleAPI.Models
{
    [Flags]
    public enum ScheduleRepeatDaysOfWeekFlags
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Everyday = Weekdays | Saturday | Sunday
    }
}
