using Express.Recurrence.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Express.Recurrence
{
    public static class Reccurance
    {
        public static RecurrenceValues GenerateDays(DayReccurance pattern)
        {
            RecurrenceValues values;
            DailyRecurrenceSettings da;
            if (pattern.EndMode == EndMode.ByOccurance)
            {
                da = new DailyRecurrenceSettings(pattern.StartDate, Convert.ToInt32(pattern.MaxOccurance));
            }
            else
            {
                da = new DailyRecurrenceSettings(pattern.StartDate, pattern.EndDate);
            }
            if (pattern.Every == Every.NDay)
            {
                values = da.GetValues(pattern.Next);
            }
            else
            {
                values = da.GetValues(1, DailyRegenType.OnEveryWeekday);
            }
            return values;
        }

        public static RecurrenceValues GenerateWeeks(WeeklyReccurance pattern)
        {
            RecurrenceValues values;
            WeeklyRecurrenceSettings we;
            SelectedDayOfWeekValues selectedValues = new SelectedDayOfWeekValues();
            if (pattern.EndMode == EndMode.ByOccurance)
            {
                we = new WeeklyRecurrenceSettings(pattern.StartDate, pattern.MaxOccurance);
            }
            else
            {
                we = new WeeklyRecurrenceSettings(pattern.StartDate, pattern.EndDate);
            }
            selectedValues.Sunday = pattern.Sunday;
            selectedValues.Monday = pattern.Monday;
            selectedValues.Tuesday = pattern.Tuesday;
            selectedValues.Wednesday = pattern.Wednesday;
            selectedValues.Thursday = pattern.Thursday;
            selectedValues.Friday = pattern.Friday;
            selectedValues.Saturday = pattern.Saturday;
            values = we.GetValues(pattern.Next, selectedValues);
            return values;
        }

        public static RecurrenceValues GenerateMonths(MonthlyReccurance pattern)
        {
            RecurrenceValues values;
            MonthlyRecurrenceSettings mo;
            if (pattern.EndMode == EndMode.ByOccurance)
            {
                mo = new MonthlyRecurrenceSettings(pattern.StartDate, pattern.MaxOccurance);
            }
            else
            {
                mo = new MonthlyRecurrenceSettings(pattern.StartDate, pattern.EndDate);
            }

            if (pattern.Constrain == CONSTRAIN.SpecificDay)
            {
                values = mo.GetValues(pattern.DayOfMonth, pattern.Next);
            }
            else
            {
                mo.AdjustmentValue = pattern.AdjustDays;
                values = mo.GetValues((MonthlySpecificDatePartOne)pattern.LogicalStart, (MonthlySpecificDatePartTwo)pattern.LogicalDay, pattern.Next);
            }
            return values;
        }
    }
}
