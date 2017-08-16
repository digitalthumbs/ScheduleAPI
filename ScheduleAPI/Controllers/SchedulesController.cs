using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Services;
using ScheduleAPI.ViewModels;
using ScheduleAPI.Builders;
using ScheduleAPI.Models;

namespace ScheduleAPI.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class SchedulesController : Controller
    {
        ScheduleService _scheduleService;

        public SchedulesController(ScheduleService ss)
        {
            _scheduleService = ss;
        }

        [HttpGet]
        public IEnumerable<ScheduleViewModel> List([FromHeader(Name = "YM-Accept-TimeZoneId")] string timeZoneId)
        {
            var schedules = _scheduleService.GetSchedules();

            return schedules.Select(x => ScheduleBuilder.BuildFromIScheduleForTimeZoneId<ScheduleViewModel>(x, timeZoneId));
        }

        [HttpGet("/schedule/{id}")]
        public IActionResult Get(string id, [FromHeader(Name = "YM-Accept-TimeZoneId")] string timeZoneId)
        {
            var s = _scheduleService.GetSchedule(id);

            if (s == null) return NotFound();

            var svm = ScheduleBuilder.BuildFromIScheduleForTimeZoneId<ScheduleViewModel>(s, timeZoneId);

            return Ok(svm);
        }

        [HttpPost("/schedule")]
        public IActionResult Post([FromBody]ScheduleViewModel svm)
        {
            if (string.IsNullOrEmpty(svm.TimeZoneId))
            {
                return BadRequest("A TimeZoneId must be specified for the schedule");
            }

            var stzi = TimeZoneInfo.FindSystemTimeZoneById(svm.TimeZoneId);
            _scheduleService.CreateSchedule(svm, stzi);

            return StatusCode(201);
        }

        [HttpPut("/schedule/{id}")]
        public IActionResult Put(string id, [FromBody]ScheduleViewModel svm)
        {
            if (string.IsNullOrEmpty(svm.TimeZoneId))
            {
                return BadRequest("A TimeZoneId must be specified for the schedule");
            }

            var oldSchedule = _scheduleService.GetSchedule(id);
            var updatedSchedule = ScheduleBuilder.BuildFromIScheduleForTimeZoneId<Schedule>(svm, svm.TimeZoneId);

            updatedSchedule.ScheduleId = Guid.Parse(id);
            _scheduleService.SaveSchedule(updatedSchedule);

            if (oldSchedule == null) return StatusCode(201);

            return Ok();

        }

        [HttpGet("nextdelivery/from/{fromUtcDateTime:datetime}/to/{toUtcDateTime:datetime}")]
        public IEnumerable<ScheduleViewModel> GetSchedulesWithNextDeliveryInRange(DateTime fromUtcDateTime, DateTime toUtcDateTime, [FromHeader(Name = "YM-Accept-TimeZoneId")] string timeZoneId)
        {
            var schedules = _scheduleService.GetSchedulesWithNextDeliveryInRange(fromUtcDateTime, toUtcDateTime);
            var svms = schedules.Select(x => ScheduleBuilder.BuildFromIScheduleForTimeZoneId<ScheduleViewModel>(x, timeZoneId));

            return svms;
        }

        [HttpGet("timeZones")]
        public IEnumerable<string> GetTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id);
        }

        [HttpGet("/schedule/{id}/upcommingDeliveries/{occurences:int}")]
        public IActionResult GetNextDeliveriesForSchedule(string id, int occurences, [FromHeader(Name = "YM-Accept-TimeZoneId")] string timeZoneId)
        {
            var schedule = _scheduleService.GetSchedule(id);

            if (schedule == null) return NotFound();

            var tzi = string.IsNullOrEmpty(timeZoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var nextDeliveryDateTimes = tzi == null
                ? schedule.NextDeliveryDateTimes(occurences)
                : schedule.NextDeliveryDateTimes(occurences)
                    .Where(x => x.HasValue)
                    .Select(x => TimeZoneInfo.ConvertTime(x.Value, tzi))
                    .Cast<DateTime?>();

            return Ok(nextDeliveryDateTimes);
        }
    }
}