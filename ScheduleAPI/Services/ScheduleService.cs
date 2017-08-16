using ScheduleAPI.Builders;
using ScheduleAPI.Extensions;
using ScheduleAPI.Models;
using ScheduleAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleAPI.Services
{
    public class ScheduleService
    {
        static Dictionary<string, Schedule> _schedules = new Dictionary<string, Schedule>();

        internal Schedule GetSchedule(string id)
        {
            var s = _schedules.GetValueOrDefault(id);
            if (s != null)
            {
                s.ResetNextDeliveryDateTime();
            }
            return s;
        }

        internal void SaveSchedule(Schedule s)
        {
            _schedules[s.ScheduleId.ToString()] = s;
        }

        internal IEnumerable<Schedule> GetSchedulesNextToDeliver(DateTime fromUtcDate)
        {
            var utcNow = DateTime.UtcNow;
            var schedules = _schedules.Where(x => x.Value.NextDeliveryDateTime.HasValue
            && x.Value.NextDeliveryDateTime.Value > fromUtcDate
            && x.Value.NextDeliveryDateTime.Value <= utcNow)
            .Select(x => x.Value);

            return schedules;
        }

        internal void CreateSchedule(ScheduleViewModel svm, TimeZoneInfo sourceTzi)
        {
            var schedule = ScheduleBuilder.BuildFromIScheduleForTimeZoneId<Schedule>(svm, sourceTzi.Id);
            schedule.ScheduleId = Guid.NewGuid();
            SaveSchedule(schedule);
        }

        internal IEnumerable<Schedule> GetSchedules()
        {
            var schedules = _schedules.Values;
            foreach (var s in schedules) { s.ResetNextDeliveryDateTime(); }
            return schedules;
        }

        internal IEnumerable<Schedule> GetSchedulesWithNextDeliveryInRange(DateTime fromUtcDt, DateTime toUtcDt)
        {
            var schedulesInRange = GetSchedules().Where(
                x => x.NextDeliveryDateTime.HasValue
                && x.NextDeliveryDateTime.Between(fromUtcDt, toUtcDt, RangeBoundaryType.Inclusive));

            return schedulesInRange;
        }
    }
}
