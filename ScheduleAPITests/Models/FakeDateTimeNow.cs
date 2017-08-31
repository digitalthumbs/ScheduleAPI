using System;
using ScheduleAPI.Models;

namespace ScheduleAPITests.Models
{
    public class FakeDateTimeNow : IDateTimeUtcNow
    {
        public DateTime UtcNow { get; }

        public FakeDateTimeNow(DateTime utcNow)
        {
            UtcNow = utcNow;
        }
    }
}