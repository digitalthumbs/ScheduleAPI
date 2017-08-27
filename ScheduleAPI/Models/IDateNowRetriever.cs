using System;

namespace ScheduleAPI.Models
{
    public interface IDateNowRetriever
    {
        DateTime UtcNow { get; }
    }
}