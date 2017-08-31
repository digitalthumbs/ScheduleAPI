using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleAPITests.Models
{
    [TestClass]
    public class ScheduleTests
    {
        [TestMethod]
        public void CalculateWeeklyNextDeliveryDateTimeTest_SameTimeZone_WeeklyAsDailyRepeat()
        {
            var activationDate = DateTime.SpecifyKind(new DateTime(2017, 08, 27, 8, 0, 0), DateTimeKind.Utc);
            var nowDate = DateTime.SpecifyKind(new DateTime(2017, 08, 27, 9, 30, 0), DateTimeKind.Utc);
            var expectedDate = activationDate.AddDays(1);

            var schedule = new Schedule(new FakeDateTimeNow(nowDate))
            {
                TimeZoneId = "Eastern Standard Time",
                ActivateDateTime = activationDate,
                RepeatCode = ScheduleRepeatCodes.Repeat,
                RepeatTimeUnitCode = ScheduleRepeatTimeUnitCodes.Weekly,
                RepeatDaysOfWeek = ScheduleRepeatDaysOfWeekFlags.Everyday
            };

            var d = schedule.NextDeliveryDateTime;
            
            Assert.AreEqual(expectedDate, d);
        }

        [TestMethod]
        public void CalculateWeeklyNextDeliveryDateTimeTest_SameTimeZone_InitalActivateDate()
        {
            var numofDaysAhead = 0;

            var testDate = DateTime.UtcNow.AddSeconds(1);
            var testTimeZoneId = "Eastern Standard Time";
            var repeatDayofWeek = GetRepeatDayOfWeekAheadOfDate(testDate, numofDaysAhead);

            DateTime? d = GetNextDeliveryDateFromScheduleForDate(testDate, testTimeZoneId, repeatDayofWeek);

            var expectedDate = testDate;
            AssertDateTimesToMinutes(d, expectedDate);
        }

        [TestMethod]
        public void CalculateWeeklyNextDeliveryDateTimeTest_SameTimeZone_DifferentWeeklyRepeatDay_1DayAhead()
        {
            var numofDaysAhead = 1;

            var testDate = DateTime.UtcNow.AddSeconds(-1);
            var testTimeZoneId = "Eastern Standard Time";
            var repeatDayofWeek = GetRepeatDayOfWeekAheadOfDate(testDate, numofDaysAhead);

            DateTime? d = GetNextDeliveryDateFromScheduleForDate(testDate, testTimeZoneId, repeatDayofWeek);

            var expectedDate = testDate.AddDays(numofDaysAhead);
            AssertDateTimesToMinutes(d, expectedDate);
        }

        [TestMethod]
        public void CalculateWeeklyNextDeliveryDateTimeTest_SameTimeZone_DifferentWeeklyRepeatDay_2DaysAhead()
        {
            var numofDaysAhead = 2;

            var testDate = DateTime.UtcNow.AddSeconds(-1);
            var testTimeZoneId = "Eastern Standard Time";
            var repeatDayofWeek = GetRepeatDayOfWeekAheadOfDate(testDate, numofDaysAhead);

            DateTime? d = GetNextDeliveryDateFromScheduleForDate(testDate, testTimeZoneId, repeatDayofWeek);

            var expectedDate = testDate.AddDays(numofDaysAhead);
            AssertDateTimesToMinutes(d, expectedDate);
        }

        [TestMethod]
        public void CalculateWeeklyNextDeliveryDateTimeTest_SameTimeZone_DifferentWeeklyRepeatDay_3DaysAhead()
        {
            var numofDaysAhead = 3;

            var testDate = DateTime.UtcNow.AddSeconds(-1);
            var testTimeZoneId = "Eastern Standard Time";
            var repeatDayofWeek = GetRepeatDayOfWeekAheadOfDate(testDate, numofDaysAhead);

            DateTime? d = GetNextDeliveryDateFromScheduleForDate(testDate, testTimeZoneId, repeatDayofWeek);

            var expectedDate = testDate.AddDays(numofDaysAhead);
            AssertDateTimesToMinutes(d, expectedDate);
        }

        [TestMethod]
        public void CalculateWeeklyNextDeliveryDateTimeTest_SameTimeZone_DifferentWeeklyRepeatDay_4DaysAhead()
        {
            var numofDaysAhead = 4;

            var testDate = DateTime.UtcNow.AddSeconds(-1);
            var testTimeZoneId = "Eastern Standard Time";
            var repeatDayofWeek = GetRepeatDayOfWeekAheadOfDate(testDate, numofDaysAhead);

            DateTime? d = GetNextDeliveryDateFromScheduleForDate(testDate, testTimeZoneId, repeatDayofWeek);

            var expectedDate = testDate.AddDays(numofDaysAhead);
            AssertDateTimesToMinutes(d, expectedDate);
        }

        [TestMethod]
        public void CalculateWeeklyNextDeliveryDateTimeTest_SameTimeZone_DifferentWeeklyRepeatDay_5DaysAhead()
        {
            var numofDaysAhead = 5;

            var testDate = DateTime.UtcNow.AddSeconds(-1);
            var testTimeZoneId = "Eastern Standard Time";
            var repeatDayofWeek = GetRepeatDayOfWeekAheadOfDate(testDate, numofDaysAhead);

            DateTime? d = GetNextDeliveryDateFromScheduleForDate(testDate, testTimeZoneId, repeatDayofWeek);

            var expectedDate = testDate.AddDays(numofDaysAhead);
            AssertDateTimesToMinutes(d, expectedDate);
        }

        [TestMethod]
        public void CalculateWeeklyNextDeliveryDateTimeTest_SameTimeZone_DifferentWeeklyRepeatDay_6DaysAhead()
        {
            var numofDaysAhead = 6;

            var testDate = DateTime.UtcNow.AddSeconds(-1);
            var testTimeZoneId = "Eastern Standard Time";
            var repeatDayofWeek = GetRepeatDayOfWeekAheadOfDate(testDate, numofDaysAhead);

            DateTime? d = GetNextDeliveryDateFromScheduleForDate(testDate, testTimeZoneId, repeatDayofWeek);

            var expectedDate = testDate.AddDays(numofDaysAhead);
            AssertDateTimesToMinutes(d, expectedDate);
        }

        private static ScheduleRepeatDaysOfWeekFlags GetRepeatDayOfWeekAheadOfDate(DateTime fromDate, int numOfDays)
        {
            var currDayOfWeekString = fromDate.AddDays(numOfDays).DayOfWeek.ToString();
            return (ScheduleRepeatDaysOfWeekFlags)Enum.Parse(typeof(ScheduleRepeatDaysOfWeekFlags), currDayOfWeekString);
        }

        private static DateTime? GetNextDeliveryDateFromScheduleForDate(DateTime testDate, string testTimeZoneId, ScheduleRepeatDaysOfWeekFlags repeatDayofWeek)
        {
            var schedule = new Schedule()
            {
                TimeZoneId = testTimeZoneId,
                ActivateDateTime = testDate,
                RepeatCode = ScheduleRepeatCodes.Repeat,
                RepeatTimeUnitCode = ScheduleRepeatTimeUnitCodes.Weekly,
                RepeatDaysOfWeek = repeatDayofWeek

            };


            var d = schedule.NextDeliveryDateTime;
            return d;
        }

        private static void AssertDateTimesToMinutes(DateTime? d, DateTime expectedDate)
        {
            Assert.IsTrue(d.HasValue);
            Assert.AreEqual(expectedDate.Year, d.Value.Year);
            Assert.AreEqual(expectedDate.Month, d.Value.Month);
            Assert.AreEqual(expectedDate.Day, d.Value.Day);
            Assert.AreEqual(expectedDate.Hour, d.Value.Hour);
            Assert.AreEqual(expectedDate.Minute, d.Value.Minute);
        }
    }
}
