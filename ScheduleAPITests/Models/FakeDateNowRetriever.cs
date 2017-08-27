using System;
using ScheduleAPI.Models;

namespace ScheduleAPITests.Models
{
    public class FakeDateNowRetriever : IDateNowRetriever
    {
        public DateTime UtcNow { get; }

        public FakeDateNowRetriever(DateTime utcNow)
        {
            UtcNow = utcNow;
        }
    }
}