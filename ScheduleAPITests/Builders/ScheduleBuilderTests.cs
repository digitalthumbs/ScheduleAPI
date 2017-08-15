using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduleAPI.Builders;
using ScheduleAPI.Models;
using ScheduleAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleAPITests.Builders
{
    [TestClass]
    public class ScheduleViewModelBuilderTests
    {
        [TestMethod]
        public void ScheduleBuilder_ConvertsFromUtc_PositiveOffset_ConvertsDates_And_ShiftsRepeatFlagValues_Left()
        {
            var dtzi = TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time");

            var s = new Schedule()
            {
                ActivateDateTime = new DateTime(2017, 1, 1, 23, 0, 0, DateTimeKind.Utc),
                StartDateTime = new DateTime(2017, 1, 1, 23, 0, 0, DateTimeKind.Utc),
                StopDateTime = DateTime.UtcNow.AddDays(5),
                RepeatCode = ScheduleRepeatCodes.Repeat,
                RepeatTimeUnitCode = ScheduleRepeatTimeUnitCodes.Weekly,
                RepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Sunday,
                TimeZoneId = "West Asia Standard Time"
            };

            var svm = ScheduleBuilder.BuildFromISchedule<ScheduleViewModel>(s, dtzi);

            var expectedActivateDateTime = TimeZoneInfo.ConvertTime(s.ActivateDateTime.Value, dtzi);
            var expectedStartDateTime = TimeZoneInfo.ConvertTime(s.StartDateTime.Value, dtzi);
            var expectedStopDateTime = TimeZoneInfo.ConvertTime(s.StopDateTime.Value, dtzi);
            var expectedNextDeliveryDateTime = TimeZoneInfo.ConvertTime(s.NextDeliveryDateTime.Value, dtzi);
            var expectedRepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Monday;

            Assert.AreEqual(expectedActivateDateTime, svm.ActivateDateTime.Value);
            Assert.AreEqual(expectedStartDateTime, svm.StartDateTime.Value);
            Assert.AreEqual(expectedStopDateTime, svm.StopDateTime.Value);
            Assert.AreEqual(expectedNextDeliveryDateTime, svm.NextDeliveryDateTime.Value);
            Assert.AreEqual(expectedRepeatDaysOfWeek, svm.RepeatDaysOfWeek);
        }

        [TestMethod]
        public void ScheduleBuilder_ConvertsFromUtc_NegativeOffset_ShiftsDates_And_RepeatFlagValues_Right()
        {
            var dtzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            var s = new Schedule()
            {
                ActivateDateTime = new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc),
                StartDateTime = new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc),
                StopDateTime = DateTime.UtcNow.AddDays(5),
                RepeatCode = ScheduleRepeatCodes.Repeat,
                RepeatTimeUnitCode = ScheduleRepeatTimeUnitCodes.Weekly,
                RepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Sunday,
                TimeZoneId = "Eastern Standard Time"
            };

            var svm = ScheduleBuilder.BuildFromISchedule<ScheduleViewModel>(s, dtzi);

            var expectedActivateDateTime = TimeZoneInfo.ConvertTime(s.ActivateDateTime.Value, dtzi);
            var expectedStartDateTime = TimeZoneInfo.ConvertTime(s.StartDateTime.Value, dtzi);
            var expectedStopDateTime = TimeZoneInfo.ConvertTime(s.StopDateTime.Value, dtzi);
            var expectedNextDeliveryDateTime = TimeZoneInfo.ConvertTime(s.NextDeliveryDateTime.Value, dtzi);
            var expectedRepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Saturday;

            Assert.AreEqual(expectedActivateDateTime, svm.ActivateDateTime.Value);
            Assert.AreEqual(expectedStartDateTime, svm.StartDateTime.Value);
            Assert.AreEqual(expectedStopDateTime, svm.StopDateTime.Value);
            Assert.AreEqual(expectedNextDeliveryDateTime, svm.NextDeliveryDateTime.Value);
            Assert.AreEqual(expectedRepeatDaysOfWeek, svm.RepeatDaysOfWeek);
        }

        [TestMethod]
        public void ScheduleBuilder_ConvertsToUtc_PositiveOffset_ShiftsDates_And_RepeatFlagValues_Left()
        {
            var stzi = TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time");

            var svm = new ScheduleViewModel()
            {
                ActivateDateTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(2017, 1, 1, 23, 0, 0, DateTimeKind.Utc), stzi),
                StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(2017, 1, 1, 23, 0, 0, DateTimeKind.Utc), stzi),
                StopDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(5), stzi),
                RepeatCode = ScheduleRepeatCodes.Repeat,
                RepeatTimeUnitCode = ScheduleRepeatTimeUnitCodes.Weekly,
                RepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Monday,
                TimeZoneId = "West Asia Standard Time"
            };

            var s = ScheduleBuilder.BuildFromISchedule<Schedule>(svm, stzi);

            var expectedActivateDateTime = TimeZoneInfo.ConvertTimeToUtc(svm.ActivateDateTime.Value, stzi);
            var expectedStartDateTime = TimeZoneInfo.ConvertTimeToUtc(svm.StartDateTime.Value, stzi);
            var expectedStopDateTime = TimeZoneInfo.ConvertTimeToUtc(svm.StopDateTime.Value, stzi);
            //var expectedNextDeliveryDateTime = TimeZoneInfo.ConvertTime(svm.NextDeliveryDateTime.Value, dtzi);
            var expectedRepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Sunday;

            Assert.AreEqual(expectedActivateDateTime, s.ActivateDateTime.Value);
            Assert.AreEqual(expectedStartDateTime, s.StartDateTime.Value);
            Assert.AreEqual(expectedStopDateTime, s.StopDateTime.Value);
            //Assert.AreEqual(expectedNextDeliveryDateTime, s.NextDeliveryDateTime.Value);
            Assert.AreEqual(expectedRepeatDaysOfWeek, s.RepeatDaysOfWeek);
        }


        [TestMethod]
        public void ScheduleBuilder_ConvertsToUtc_NegativeOffset_ShiftsDates_And_RepeatFlagValues_Right()
        {
            var stzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            var svm = new ScheduleViewModel()
            {
                ActivateDateTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc), stzi),
                StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc), stzi),
                StopDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(5), stzi),
                RepeatCode = ScheduleRepeatCodes.Repeat,
                RepeatTimeUnitCode = ScheduleRepeatTimeUnitCodes.Weekly,
                RepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Saturday,
                TimeZoneId = "Eastern Standard Time"
            };

            var s = ScheduleBuilder.BuildFromISchedule<Schedule>(svm, stzi);

            var expectedActivateDateTime = TimeZoneInfo.ConvertTimeToUtc(svm.ActivateDateTime.Value, stzi);
            var expectedStartDateTime = TimeZoneInfo.ConvertTimeToUtc(svm.StartDateTime.Value, stzi);
            var expectedStopDateTime = TimeZoneInfo.ConvertTimeToUtc(svm.StopDateTime.Value, stzi);
            //var expectedNextDeliveryDateTime = TimeZoneInfo.ConvertTime(svm.NextDeliveryDateTime.Value, dtzi);
            var expectedRepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Sunday;

            Assert.AreEqual(expectedActivateDateTime, s.ActivateDateTime.Value);
            Assert.AreEqual(expectedStartDateTime, s.StartDateTime.Value);
            Assert.AreEqual(expectedStopDateTime, s.StopDateTime.Value);
            //Assert.AreEqual(expectedNextDeliveryDateTime, s.NextDeliveryDateTime.Value);
            Assert.AreEqual(expectedRepeatDaysOfWeek, s.RepeatDaysOfWeek);
        }
    }
}
