using System;

namespace ScheduleAPI.Models
{
    public sealed class DateNowRetriever : IDateNowRetriever
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}