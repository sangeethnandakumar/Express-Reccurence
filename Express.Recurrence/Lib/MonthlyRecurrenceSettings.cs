using System;

namespace Express.Recurrence.Lib
{
    #region Class-specific Enums
    /// <summary>
    /// The Regeneration type. Is this on a specific day of the month, a custom date, or after the occurrence is completed.
    /// </summary>
    public enum MonthlyRegenType { NotSet = -1, OnSpecificDayOfMonth, OnCustomDateFormat, AfterOccurrenceCompleted };
    /// <summary>
    /// First part of a custom date. This would be First, Second, etc. item of the month.
    /// </summary>
    public enum MonthlySpecificDatePartOne { NotSet = -1, First, Second, Third, Fourth, Last };
    /// <summary>
    /// Second part of a custom date. This is day of week, weekend day, etc.
    /// </summary>
    public enum MonthlySpecificDatePartTwo { NotSet = -1, Day, Weekday, WeekendDay, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };

#endregion //Class-specific Enums

    public class MonthlyRecurrenceSettings : RecurrenceSettings
    {
        #region Constructors
        /// <summary>
        /// Get dates by Start date only. This is for no ending date values.
        /// </summary>
        /// <param name="startDate"></param>
        public MonthlyRecurrenceSettings(DateTime startDate) : base(startDate) { }
        /// <summary>
        /// Get dates by Start and End date boundries.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public MonthlyRecurrenceSettings(DateTime startDate, DateTime endDate): base(startDate, endDate){}
        /// <summary>
        /// Get dates by Start date and number of occurrences.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="numberOfOccurrences"></param>
        public MonthlyRecurrenceSettings(DateTime startDate, int numberOfOccurrences): base(startDate, numberOfOccurrences){ }
        #endregion

        #region Private Fields
        int regenerateOnSpecificDateDayValue;
        int regenEveryXMonths = 1;
        int adjustmentValue;
        bool getNextDateValue;
        DateTime nextDateValue;
        MonthlyRegenType regenType = MonthlyRegenType.NotSet;
        MonthlySpecificDatePartOne specificDatePartOne = MonthlySpecificDatePartOne.NotSet;
        MonthlySpecificDatePartTwo specificDatePartTwo = MonthlySpecificDatePartTwo.NotSet;
        #endregion

        #region Private Procedures
        /// <summary>
        /// Get the Series information that's used to get dates at a later
        /// date. This is passed into the RecurrenceHelper to get date values.
        /// Most likely used for non-ending dates.
        /// </summary>
        /// <returns></returns>
        string GetSeriesInfo()
        {
            string info;
            string endDate = "ZZZZZZZZ"; // Default for no ending date.
            string occurrences = base.NumberOfOccurrences.ToString("0000");
            string monthlyRegenType = ((int)regenType).ToString("0");
            string endDateType = ((int)base.TypeOfEndDate).ToString("0");
            string regenOnSpecificDateDayValue = regenerateOnSpecificDateDayValue.ToString("00");
            string specDatePartOne = "Z";
            string specDatePartTwo = "Z";
            string regenXMonths = regenEveryXMonths.ToString("000");

            if (specificDatePartOne != MonthlySpecificDatePartOne.NotSet)
                specDatePartOne = Convert.ToString((char)(int)(specificDatePartOne + 65));

            if (specificDatePartTwo != MonthlySpecificDatePartTwo.NotSet)
                specDatePartTwo = Convert.ToString((char)(int)(specificDatePartTwo + 65));

            string adjustValue = adjustmentValue.ToString("000");

            // If the Adjustment value is less than Zero then account for the "-"
            if (adjustmentValue < 0)
                adjustValue = adjustmentValue.ToString("00");

            // End Date may be null if no ending date.
            if (base.EndDate.HasValue)
                endDate = base.EndDate.Value.ToString("yyyyMMdd");

            // FORMATTING DEFINITIONS
            //  Y = Yearly Designator
            //  |       0 = Start Date (8 chars)
            //  |       |        1 = End Date (8 chars)
            //  |       |        |        2 = Number of occurrences (4 chars)
            //  |       |        |        |      3 = Regeneration Type (1 char)
            //  |       |        |        |      |    4 = End date type (1 char)
            //  |       |        |        |      |    |      5 = Regenerate on specific date DAY value (2 chars)
            //  |       |        |        |      |    |      |       6 = Custom Date Part One (1 char)
            //  |       |        |        |      |    |      |       |       7 = Custom Date Part Two (1 char)
            //  |       |        |        |      |    |      |       |       |       8 = Adjustment Value (3 chars)
            //  |       |        |        |      |    |      |       |       |       |     9  Regen Every x months
            //  |       |        |        |      |    |      |       |       |       |     |  
            // [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-24]  [25]    [26]    [27-29] [30-32]
            //  M   20071231  20171231   0000    1     1     00      A      A       000    000
            info = string.Format("M{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", base.StartDate.ToString("yyyyMMdd"), endDate, occurrences, monthlyRegenType, endDateType, regenOnSpecificDateDayValue, specDatePartOne, specDatePartTwo, adjustValue, regenXMonths);
            return info;
        }


        /// <summary>
        /// Get recurring dates from a specific constant date such as 27 July.
        /// </summary>
        /// <returns></returns>
        RecurrenceValues GetSpecificDayOfMonthDates()
        {
            RecurrenceValues values = new RecurrenceValues();
            DateTime dt = base.StartDate;
            int dayValue = regenerateOnSpecificDateDayValue;
            int daysOfMonth = DateTime.DaysInMonth(dt.Year, dt.Month);
            // Get the max days of the month and make sure it's not 
            // less than the specified day value trying to be set.
            if (daysOfMonth < regenerateOnSpecificDateDayValue)
                dayValue = daysOfMonth;

            // Determine if start date is greater than the Day and Month values
            // for a specific date.
            DateTime newDate = new DateTime(dt.Year, dt.Month, dayValue);
            // Is the specific date before the start date, if so 
            // then make the specific date next month.
            if (newDate < dt)
                dt = newDate.AddMonths(1);
            else
                dt = newDate;


            if (getNextDateValue)
            {
                do
                {
                    values.AddDateValue(dt, adjustmentValue);
                    if (values.Values[values.Values.Count - 1] > nextDateValue)
                        break;

                    dt = dt.AddMonths(RegenEveryXMonths);
                    dt = GetCorrectedDate(dt);
                } while (dt <= nextDateValue.AddMonths(RegenEveryXMonths + 1)); // Ensure the last date if greater than what's needed for the next date in the cycle
            }
            else
            {
                switch (base.TypeOfEndDate)
                {
                    case EndDateType.NoEndDate:
                        throw new Exception("The ability to create recurring dates with no End date is not currently available.");

                    case EndDateType.NumberOfOccurrences:

                        for (int i = 0; i < base.NumberOfOccurrences; i++)
                        {
                            values.AddDateValue(dt, adjustmentValue);
                            dt = dt.AddMonths(RegenEveryXMonths);
                            dt = GetCorrectedDate(dt);
                        }
                        break;

                    case EndDateType.SpecificDate:
                        do
                        {
                            values.AddDateValue(dt, adjustmentValue);
                            dt = dt.AddMonths(RegenEveryXMonths);
                            dt = GetCorrectedDate(dt);
                        } while (dt <= base.EndDate);
                        break;

                    default:
                        throw new ArgumentNullException("TypeOfEndDate", "The TypeOfEndDate property has not been set.");
                }
            }

            return values;
        }

        /// <summary>
        /// Correct an input date to be equal to or less than the specified day value.
        /// </summary>
        /// <param name="input">Date to check to ensure it matches the specified day value or the max number of days for that month, whichever comes first.</param>
        /// <returns>DateTime</returns>
        DateTime GetCorrectedDate(DateTime input)
        {
            DateTime dt = input;
            // Ensure the day value hasn't changed.
            // This will occurr if the month is Feb. All
            // dates after that will have the same day.
            if (dt.Day < this.regenerateOnSpecificDateDayValue && DateTime.DaysInMonth(dt.Year, dt.Month) > dt.Day)
            {
                // The Specified day is greater than the number of days in the month.
                if (this.regenerateOnSpecificDateDayValue > DateTime.DaysInMonth(dt.Year, dt.Month))
                    dt = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
                else
                    // The specified date is less than number of days in month.
                    dt = new DateTime(dt.Year, dt.Month, this.regenerateOnSpecificDateDayValue);
            }
            return dt;
        }

        /// <summary>
        /// Get recurring dates from custom date such as First Sunday of July.
        /// </summary>
        /// <returns></returns>
        RecurrenceValues GetCustomDayOfMonthDates()
        {
            if (this.SpecificDatePartOne == MonthlySpecificDatePartOne.NotSet)
                throw new Exception("The First part of the custom date has not been set.");
            if (this.SpecificDatePartTwo == MonthlySpecificDatePartTwo.NotSet)
                throw new Exception("The Second part of the custom date has not been set.");

            RecurrenceValues values = new RecurrenceValues();
            DateTime dt = base.StartDate;

            // Get Next Date value only
            if (getNextDateValue)
            {
                do
                {
                    dt = GetCustomDate(dt);
                    // If the date returned is less than the start date
                    // then do it again to increment past the start date
                    if (dt < base.StartDate)
                    {
                        dt = dt.AddMonths(1);
                        dt = GetCustomDate(dt);
                    }
                    values.AddDateValue(dt, adjustmentValue);
                    if (values.Values[values.Values.Count - 1] > nextDateValue)
                        break;

                    dt = dt.AddMonths(RegenEveryXMonths);

                } while (dt <= nextDateValue.AddMonths(RegenEveryXMonths + 1)); // Ensure the last date if greater than what's needed for the next date in the cycle
            }
            else
            {
                switch (base.TypeOfEndDate)
                {
                    case EndDateType.NoEndDate:
                        throw new Exception("The ability to create recurring dates with no End date is not currently available.");

                    case EndDateType.NumberOfOccurrences:
                        for (int i = 0; i < base.NumberOfOccurrences; i++)
                        {
                            dt = GetCustomDate(dt);
                            // If the date returned is less than the start date
                            // then do it again to increment past the start date
                            if (dt < base.StartDate)
                            {
                                dt = dt.AddMonths(1);
                                dt = GetCustomDate(dt);
                            }
                            values.AddDateValue(dt, adjustmentValue);
                            dt = dt.AddMonths(RegenEveryXMonths);
                        }
                        break;

                    case EndDateType.SpecificDate:
                        do
                        {
                            dt = GetCustomDate(dt);
                            // If the date returned is less than the start date
                            // then do it again to increment past the start date
                            if (dt < base.StartDate)
                            {
                                dt = dt.AddMonths(1);
                                dt = GetCustomDate(dt);
                            }
                            values.AddDateValue(dt, adjustmentValue);
                            dt = dt.AddMonths(RegenEveryXMonths);
                        } while (dt <= base.EndDate);
                        break;

                    default:
                        throw new ArgumentNullException("TypeOfEndDate", "The TypeOfEndDate property has not been set.");
                }
            }
            return values;

        }

        /// <summary>
        /// Get the custom value from the 1st, 2nd, and 3rd custom date parts
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        DateTime GetCustomDate(DateTime currentDate)
        {
            int year = currentDate.Year;
            DateTime dt = new DateTime(year, currentDate.Month, 1);
            int day = 1;
            int firstPart = (int)SpecificDatePartOne + 1;
            int daysOfMonth = DateTime.DaysInMonth(year, dt.Month);

            switch (SpecificDatePartTwo)
            {
                case MonthlySpecificDatePartTwo.Day:
                    // If only getting the Last day of the month
                    if (SpecificDatePartOne == MonthlySpecificDatePartOne.Last)
                        dt = new DateTime(year, dt.Month, DateTime.DaysInMonth(year, dt.Month));
                    else
                        // Get a specific day of the month such as First, Second, Third, Fourth
                        dt = new DateTime(year, dt.Month, firstPart);

                    break;

                case MonthlySpecificDatePartTwo.Weekday:
                    int weekDayCount = 0;
                    DateTime lastWeekday = dt;
                    do
                    {
                        // Check for anything other than Saturday and Sunday
                        if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                        {
                            // Get a specific Weekday of the Month
                            if (SpecificDatePartOne != MonthlySpecificDatePartOne.Last)
                            {
                                // Add up the weekday count
                                weekDayCount++;
                                // If the current weekday count matches then exit
                                if (weekDayCount == firstPart)
                                    break;
                            }
                            else
                            {
                                // Get the last weekday of the month
                                lastWeekday = dt;
                            }
                        }
                        dt = dt.AddDays(1);
                        day++;
                    } while (day <= daysOfMonth);

                    // If getting the last weekday of the month then 
                    // set the returning value to the last weekday found.
                    if (SpecificDatePartOne == MonthlySpecificDatePartOne.Last)
                        dt = lastWeekday;

                    break;

                case MonthlySpecificDatePartTwo.WeekendDay:
                    int weekendDayCount = 0;
                    DateTime lastWeekendday = dt;
                    do
                    {
                        // Check for anything other than Saturday and Sunday
                        if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                        {
                            // Get a specific Weekday of the Month
                            if (SpecificDatePartOne != MonthlySpecificDatePartOne.Last)
                            {
                                // Add up the weekday count
                                weekendDayCount++;
                                // If the current weekday count matches then exit
                                if (weekendDayCount == firstPart)
                                    break;
                            }
                            else
                            {
                                // Get the last weekday of the month
                                lastWeekendday = dt;
                            }
                        }
                        dt = dt.AddDays(1);
                        day++;
                    } while (day <= daysOfMonth);

                    // If getting the last weekday of the month then 
                    // set the returning value to the last weekday found.
                    if (SpecificDatePartOne == MonthlySpecificDatePartOne.Last)
                        dt = lastWeekendday;

                    break;

                case MonthlySpecificDatePartTwo.Monday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Monday, daysOfMonth, firstPart);
                    break;

                case MonthlySpecificDatePartTwo.Tuesday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Tuesday, daysOfMonth, firstPart);
                    break;

                case MonthlySpecificDatePartTwo.Wednesday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Wednesday, daysOfMonth, firstPart);
                    break;

                case MonthlySpecificDatePartTwo.Thursday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Thursday, daysOfMonth, firstPart);
                    break;

                case MonthlySpecificDatePartTwo.Friday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Friday, daysOfMonth, firstPart);
                    break;

                case MonthlySpecificDatePartTwo.Saturday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Saturday, daysOfMonth, firstPart);
                    break;

                case MonthlySpecificDatePartTwo.Sunday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Sunday, daysOfMonth, firstPart);
                    break;
            }
            return dt;
        }

        DateTime GetCustomWeekday(DateTime startDate, DayOfWeek weekDay, int daysOfMonth, int firstDatePart)
        {
            int day = 1;
            int dayCount = 0;
            DateTime lastDOW = startDate;
            DateTime returnDate = startDate;
            do
            {
                // Check for day of the week
                if (returnDate.DayOfWeek == weekDay)
                {
                    // Get a specific Weekday of the Month
                    if (SpecificDatePartOne != MonthlySpecificDatePartOne.Last)
                    {
                        // Add up the days found count
                        dayCount++;
                        // If the current weekday count matches then exit
                        if (dayCount == firstDatePart)
                        {
                            break;
                        }
                    }
                    else
                    {
                        // Get the current date value
                        lastDOW = returnDate;
                    }
                }
                returnDate = returnDate.AddDays(1);
                day++;
            } while (day <= daysOfMonth);

            // If getting the last weekday of the month then 
            // set the returning value to the last weekday found.
            if (SpecificDatePartOne == MonthlySpecificDatePartOne.Last)
                returnDate = lastDOW;

            return returnDate;
        }
        #endregion //Private Procedures

        /// <summary>
        ///     Get a user-friendly class that is a easy way using Properties that define the Series Info
        /// </summary>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         String of Series Info that was first generated by this MonthlyRecurrenceSettings Class object.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A RecurrenceGenerator.RecurrenceInfo value...
        /// </returns>
        internal static RecurrenceInfo GetFriendlyRecurrenceInfo(string seriesInfo)
        {
            RecurrenceInfo info = new RecurrenceInfo();
            EndDateType endType = EndDateType.NotDefined;
            DateTime endDateValue = DateTime.MinValue;
            int noOccurrences;

            // Exit if not a Monthly seriesInfo type
            if (!seriesInfo.StartsWith("M"))
                return null;

            info.SetSeriesInfo(seriesInfo);
            info.SetRecurrenceType(RecurrenceType.Monthly);
            // FORMATTING DEFINITIONS
            //  Y = Yearly Designator
            //  |       0 = Start Date (8 chars)
            //  |       |        1 = End Date (8 chars)
            //  |       |        |        2 = Number of occurrences (4 chars)
            //  |       |        |        |      3 = Regeneration Type (1 char)
            //  |       |        |        |      |    4 = End date type (1 char)
            //  |       |        |        |      |    |      5 = Regenerate on specific date DAY value (2 chars)
            //  |       |        |        |      |    |      |       6 = Custom Date Part One (1 char)
            //  |       |        |        |      |    |      |       |       7 = Custom Date Part Two (1 char)
            //  |       |        |        |      |    |      |       |       |       8 = Adjustment Value (3 chars)
            //  |       |        |        |      |    |      |       |       |       |     9  Regen Every x months
            //  |       |        |        |      |    |      |       |       |       |     |  
            // [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-24]  [25]    [26]    [27-29] [30-32]
            //  M   01082007  20171231   0000    1     1     00      A      A       000    000
            string startDate = seriesInfo.Substring(1, 8);

            DateTime dtStartDate = new DateTime(int.Parse(startDate.Substring(0, 4)), int.Parse(startDate.Substring(4, 2)), int.Parse(startDate.Substring(6, 2)));
            string endDate = seriesInfo.Substring(9, 8);

            string occurrences = seriesInfo.Substring(17, 4);
            string monthlyRegenType = seriesInfo.Substring(21, 1);
            string endDateType = seriesInfo.Substring(22, 1);
            string regenOnSpecificDateDayValue = seriesInfo.Substring(23, 2);
            string specDatePartOne = seriesInfo.Substring(25, 1);
            string specDatePartTwo = seriesInfo.Substring(26, 1);
            string adjustValue = seriesInfo.Substring(27, 3);
            int regenXMonths = int.Parse(seriesInfo.Substring(30, 3));
            endType = (EndDateType)(int.Parse(endDateType));
            noOccurrences = int.Parse(occurrences);

            MonthlySpecificDatePartOne partOne = MonthlySpecificDatePartOne.NotSet;
            MonthlySpecificDatePartTwo partTwo = MonthlySpecificDatePartTwo.NotSet;

            if (specDatePartOne == "Z")
                partOne = MonthlySpecificDatePartOne.NotSet;
            else
                partOne = (MonthlySpecificDatePartOne)(Convert.ToInt32(specDatePartOne[0]) - 65);

            if (specDatePartTwo == "Z")
                partTwo = MonthlySpecificDatePartTwo.NotSet;
            else
                partTwo = (MonthlySpecificDatePartTwo)(Convert.ToInt32(specDatePartTwo[0]) - 65);

            endType = (EndDateType)(int.Parse(endDateType));
            noOccurrences = int.Parse(occurrences);

            // Get the EndDate before any modifications on it are performed
            if (endType == EndDateType.SpecificDate)
                endDateValue = new DateTime(int.Parse(endDate.Substring(0, 4)), int.Parse(endDate.Substring(4, 2)), int.Parse(endDate.Substring(6, 2)));

            info.SetEndDateType(endType);
            // Determine the Constructor by the type of End Date.
            // All constructors start with a Start date at a minimum.
            switch (endType)
            {
                case EndDateType.NumberOfOccurrences:
                    info.SetStartDate(dtStartDate);
                    info.SetNumberOfOccurrences(noOccurrences);
                    break;

                case EndDateType.SpecificDate:
                    info.SetStartDate(dtStartDate);
                    info.SetEndDate(endDateValue);
                    break;

                case EndDateType.NoEndDate:
                    info.SetStartDate(dtStartDate);
                    break;
            }

            // Set the adjusted values
            info.SetAdjustmentValue(Convert.ToInt32(adjustValue));
            info.SetMonthlyRegenType((MonthlyRegenType)(int.Parse(monthlyRegenType)));

            // Determine the Type of dates to get, specific, custom, etc.
            switch (info.MonthlyRegenType)
            {
                case MonthlyRegenType.OnSpecificDayOfMonth:
                    info.SetMonthlyRegenerateOnSpecificDateDayValue(int.Parse(regenOnSpecificDateDayValue));
                    info.SetRegenEveryXMonths(regenXMonths);
                    break;

                case MonthlyRegenType.OnCustomDateFormat:
                    info.SetMonthlySpecificDatePartOne(partOne);
                    info.SetMonthlySpecificDatePartTwo(partTwo);
                    info.SetRegenEveryXMonths(regenXMonths);
                    break;

                case MonthlyRegenType.AfterOccurrenceCompleted:

                    break;
            }

            return info;
        }

        #region Public GetValues
        /// <summary>
        /// Get Custom dates such as Last Saturday of the month with option as to the increment of every x-months.
        /// </summary>
        /// <param name="customDatePartOne">Corresponds to Part of month such as First, Last.</param>
        /// <param name="customDatePartTwo">Corresponds to day of the week to get such as Tuesday or Weekend Day.</param>
        /// <param name="regenEveryXMonths">How many months to skip, such as 2 for every other month.</param>
        /// <returns></returns>
        public RecurrenceValues GetValues(MonthlySpecificDatePartOne customDatePartOne, MonthlySpecificDatePartTwo customDatePartTwo, int regenEveryXMonths)
        {
            this.regenEveryXMonths = regenEveryXMonths;
            specificDatePartOne = customDatePartOne;
            specificDatePartTwo = customDatePartTwo;
            regenType = MonthlyRegenType.OnCustomDateFormat;
            return GetValues();
        }

        /// <summary>
        /// Get values for a specific day of the month. Eg. Every 23rd day. With option to get every x-month.
        /// </summary>
        /// <param name="dayOfMonthToRegen">Day of month you want to set as the value to get from month to month.</param>
        /// <param name="regenEveryXMonths">How many months to skip, such as 2 for every other month.</param>
        /// <returns></returns>
        public RecurrenceValues GetValues(int dayOfMonthToRegen, int regenEveryXMonths)
        {
            this.regenEveryXMonths = regenEveryXMonths;
            regenerateOnSpecificDateDayValue = dayOfMonthToRegen;
            regenType = MonthlyRegenType.OnSpecificDayOfMonth;
            return GetValues();
        }
#endregion //Public GetValues

        #region Public Fields
        /// <summary>
        /// What is the first part to the Custom date such as First, Last.
        /// </summary>
        public MonthlySpecificDatePartOne SpecificDatePartOne
        {
            get
            {
                return specificDatePartOne;
            }
        }

        /// <summary>
        /// What is the second part to the Custom date such as which weekday, weekend day, etc.
        /// </summary>
        public MonthlySpecificDatePartTwo SpecificDatePartTwo
        {
            get
            {
                return specificDatePartTwo;
            }
        }

        /// <summary>
        /// What is the regeneration type such as Specific day of month, custom date, etc.
        /// </summary>
        public MonthlyRegenType RegenType
        {
            get
            {
                return regenType;
            }
        }

        /// <summary>
        ///  Day of month to regenerate when RegenType = specific day of month.
        /// </summary>
        public int RegenerateOnSpecificDateDayValue
        {
            get
            {
                return regenerateOnSpecificDateDayValue;
            }
        }

        /// <summary>
        /// Used to adjust the date plus/minus x-days
        /// </summary>
        public int AdjustmentValue
        {
            get
            {
                return adjustmentValue;
            }
            set
            {
                adjustmentValue = value;
            }
        }
        /// <summary>
        /// What is the interval to generate dates. This is used to skip months in the cycle.
        /// </summary>
        public int RegenEveryXMonths
        {
            get
            {
                return regenEveryXMonths;
            }
        }
        #endregion //Public Fields

        #region Internal GetValues
        /// <summary>
        ///      Final overloaded function that gets the Recurrence Values. 
        ///      This is called from the RecurrenceHelper staic methods only.
        /// </summary>
        /// <returns>
        ///     A RecurrenceGenerator.RecurrenceValues value...
        /// </returns>
        internal override RecurrenceValues GetValues()
        {
            return GetRecurrenceValues();
        }
        internal override RecurrenceValues GetValues(DateTime startDate, DateTime endDate)
        {
            base.StartDate = startDate;
            base.EndDate = endDate;
            // Change the end type to End Date as this original series info
            // may have been set to number of occurrences.
            base.endDateType = EndDateType.SpecificDate;
            return GetRecurrenceValues();
        }

        internal override RecurrenceValues GetValues(DateTime startDate, int numberOfOccurrences)
        {
            base.NumberOfOccurrences = numberOfOccurrences;
            base.StartDate = startDate;
            // Change the end type to number of occurrences. 
            // This must be set because the original starting Series Info may
            // be set to have an End Date type.
            base.endDateType = EndDateType.NumberOfOccurrences;
            return GetRecurrenceValues();
        }

        RecurrenceValues GetRecurrenceValues()
        {
            RecurrenceValues values = null;
            switch (RegenType)
            {
                case MonthlyRegenType.OnSpecificDayOfMonth:
                    values = GetSpecificDayOfMonthDates();
                    break;

                case MonthlyRegenType.OnCustomDateFormat:
                    values = GetCustomDayOfMonthDates();
                    break;

                case MonthlyRegenType.AfterOccurrenceCompleted:

                    break;

            }
            if (values.Values.Count > 0)
            {
                values.SetStartDate(values.Values[0]);

                // Get the end date if not open-ended
                if (base.TypeOfEndDate != EndDateType.NoEndDate)
                    values.SetEndDate(values.Values[values.Values.Count - 1]);
            }

            // Set the Series information that's used to get the next date
            // values for no ending dates.
            values.SetSeriesInfo(GetSeriesInfo());

            return values;
        }
#endregion //Internal GetValues
        
        #region Internal Procedures
        internal static string GetPatternDefinition(string seriesInfo)
        {
            string returnValue = string.Empty;
            returnValue =
            " MONTHLY FORMATTING DEFINITIONS\r\n" +
            "  Y = Yearly Designator\r\n" +
            "  |       0 = Start Date (8 chars)\r\n" +
            "  |       |        1 = End Date (8 chars)\r\n" +
            "  |       |        |        2 = Number of occurrences (4 chars)\r\n" +
            "  |       |        |        |      3 = Regeneration Type (1 char)\r\n" +
            "  |       |        |        |      |    4 = End date type (1 char)\r\n" +
            "  |       |        |        |      |    |      5 = Regenerate on specific date DAY value (2 chars)\r\n" +
            "  |       |        |        |      |    |      |       6 = Custom Date Part One (1 char)\r\n" +
            "  |       |        |        |      |    |      |       |       7 = Custom Date Part Two (1 char)\r\n" +
            "  |       |        |        |      |    |      |       |       |       8 = Adjustment Value (3 chars)\r\n" +
            "  |       |        |        |      |    |      |       |       |       |     9  Regen Every x months\r\n" +
            "  |       |        |        |      |    |      |       |       |       |     |  \r\n" +
            " [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-24]  [25]    [26]    [27-29] [30-32]\r\n" +
            string.Format("  M    {0} {1}  {2}     {3}    {4}     {5}      {6}       {7}        {8}     {9}\r\n", seriesInfo.Substring(1, 8), seriesInfo.Substring(9, 8), seriesInfo.Substring(17, 4), seriesInfo.Substring(21, 1), seriesInfo.Substring(22, 1), seriesInfo.Substring(23, 2), seriesInfo.Substring(25, 1), seriesInfo.Substring(26, 1), seriesInfo.Substring(27, 3), seriesInfo.Substring(30, 3));

            return returnValue;
        }

        void SetValues(MonthlySpecificDatePartOne customDatePartOne, MonthlySpecificDatePartTwo customDatePartTwo, int regenEveryXMonths)
        {
            this.regenEveryXMonths = regenEveryXMonths;
            specificDatePartOne = customDatePartOne;
            specificDatePartTwo = customDatePartTwo;
            regenType = MonthlyRegenType.OnCustomDateFormat;
        }

        void SetValues(int dayOfMonthToRegen, int regenEveryXMonths)
        {
            this.regenEveryXMonths = regenEveryXMonths;
            regenerateOnSpecificDateDayValue = dayOfMonthToRegen;
            regenType = MonthlyRegenType.OnSpecificDayOfMonth;
        }


        internal override DateTime GetNextDate(DateTime currentDate)
        {
            getNextDateValue = true;
            nextDateValue = currentDate;
            // Now get the values and return the last date found.
            RecurrenceValues values = GetValues();
            return values.EndDate;
        }

        internal static MonthlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo)
        {
            return GetRecurrenceSettings(seriesInfo, -1, DateTime.MinValue);
        }
        internal static MonthlyRecurrenceSettings GetRecurrenceSettings(DateTime modifiedStartDate, string seriesInfo)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedStartDate, DateTime.MinValue);
        }

        internal static MonthlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue)
        {
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, DateTime.MinValue);
        }
        internal static MonthlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedStartDate, int modifiedOccurrencesValue)
        {
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, modifiedStartDate, DateTime.MinValue);
        }


        internal static MonthlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedEndDate)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedEndDate);
        }
        internal static MonthlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedStartDate, DateTime modifiedEndDate)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedStartDate, modifiedEndDate);
        }

        static MonthlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue, DateTime modifiedStartDate, DateTime modifiedEndDate)
        {
            // Get the Recurrence Info object. This makes it easy to work with existing series of date patterns.
            RecurrenceInfo info = MonthlyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);
            MonthlyRecurrenceSettings settings = null;

            // Check to see if this is to modify the SeriesInfo and run as endtype for occurrences
            if (modifiedOccurrencesValue != -1)
            {
                info.SetEndDateType(EndDateType.NumberOfOccurrences);
                info.SetNumberOfOccurrences(modifiedOccurrencesValue);
            }
            // Check to see if this is to modify the EndDate and run as endType for EndDate
            if (modifiedEndDate != DateTime.MinValue)
            {
                info.SetEndDateType(EndDateType.SpecificDate);
                info.SetEndDate(modifiedEndDate);
            }

            // Determine the Constructor by the type of End Date.
            // All constructors start with a Start date at a minimum.
            switch (info.EndDateType)
            {
                case EndDateType.NumberOfOccurrences:
                    settings = new MonthlyRecurrenceSettings(modifiedStartDate, info.NumberOfOccurrences);
                    break;

                case EndDateType.SpecificDate:
                    settings = new MonthlyRecurrenceSettings(modifiedStartDate, info.EndDate.Value);
                    break;

                case EndDateType.NoEndDate:
                    settings = new MonthlyRecurrenceSettings(modifiedStartDate);
                    break;
            }

            settings.AdjustmentValue = info.AdjustmentValue;

            // Determine the Type of dates to get, specific, custom, etc.
            switch (info.MonthlyRegenType)
            {
                case MonthlyRegenType.OnSpecificDayOfMonth:
                    settings.SetValues(info.MonthlyRegenerateOnSpecificDateDayValue, info.MonthlyRegenEveryXMonths);
                    break;

                case MonthlyRegenType.OnCustomDateFormat:
                    settings.SetValues(info.MonthlySpecificDatePartOne, info.MonthlySpecificDatePartTwo, info.MonthlyRegenEveryXMonths);
                    break;

                case MonthlyRegenType.AfterOccurrenceCompleted:

                    break;
            }

            return settings;
        }

        static MonthlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue, DateTime modifiedEndDate)
        {
            // Get the Recurrence Info object. This makes it easy to work with existing series of date patterns.
            RecurrenceInfo info = MonthlyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, info.StartDate, modifiedEndDate);
        }
#endregion //Internal Procedures

        
        

    }
}
