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
            TimeZoneInfo dtzi = null;
            if (timeZoneId != null)
            {
                dtzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }

            var schedules = _scheduleService.GetSchedules();

            return schedules.Select(x => ScheduleBuilder.BuildFromISchedule<ScheduleViewModel>(x, dtzi));
        }

        [HttpGet("/schedule/{id}")]
        public IActionResult Get(string id, [FromHeader(Name = "YM-Accept-TimeZoneId")] string timeZoneId)
        {
            TimeZoneInfo dtzi = null;
            if (timeZoneId != null)
            {
                dtzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }

            var s = _scheduleService.GetSchedule(id);

            if (s == null) return NotFound();

            var svm = ScheduleBuilder.BuildFromISchedule<ScheduleViewModel>(s, dtzi);

            return Ok(svm);
        }

        [HttpPost("/schedule")]
        public IActionResult Post([FromBody]ScheduleViewModel svm)
        {
            var stzi = TimeZoneInfo.FindSystemTimeZoneById(svm.TimeZoneId);
            _scheduleService.CreateSchedule(svm, stzi);

            return StatusCode(201);
        }

        [HttpPut("/schedule/{id}")]
        public IActionResult Put(string id, [FromBody]ScheduleViewModel svm)
        {
            var stzi = TimeZoneInfo.FindSystemTimeZoneById(svm.TimeZoneId);

            var oldSchedule = _scheduleService.GetSchedule(id);
            var updatedSchedule = ScheduleBuilder.BuildFromISchedule<Schedule>(svm, stzi);

            updatedSchedule.ScheduleId = Guid.Parse(id);
            _scheduleService.SaveSchedule(updatedSchedule);

            if (oldSchedule == null) return StatusCode(201);

            return Ok();

        }

        [HttpGet("nextdelivery/from/{fromUtcDateTime:datetime}/to/{toUtcDateTime:datetime}")]
        public IEnumerable<ScheduleViewModel> GetSchedulesWithNextDeliveryInRange(DateTime fromUtcDateTime, DateTime toUtcDateTime, [FromHeader(Name = "YM-Accept-TimeZoneId")] string timeZoneId)
        {
            var dtzi = timeZoneId == null ? TimeZoneInfo.Utc : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var schedules = _scheduleService.GetSchedulesWithNextDeliveryInRange(fromUtcDateTime, toUtcDateTime);
            var svms = schedules.Select(x => ScheduleBuilder.BuildFromISchedule<ScheduleViewModel>(x, dtzi));

            return svms;
        }

        [HttpGet("timeZones")]
        public IEnumerable<string> GetTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id);
        }
    }
}