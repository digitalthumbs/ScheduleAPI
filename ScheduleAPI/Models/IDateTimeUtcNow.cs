using System;

namespace ScheduleAPI.Models
{
    public interface IDateTimeUtcNow
    {
        DateTime UtcNow { get; }
    }
}