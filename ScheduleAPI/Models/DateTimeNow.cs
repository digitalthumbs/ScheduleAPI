using System;

namespace ScheduleAPI.Models
{
    public sealed class DateTimeNow : IDateTimeUtcNow, IDateTimeNow
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}