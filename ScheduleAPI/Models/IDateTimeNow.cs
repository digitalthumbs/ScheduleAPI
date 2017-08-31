using System;

namespace ScheduleAPI.Models
{
    public interface IDateTimeNow
    {
        DateTime Now { get; }
    }
}