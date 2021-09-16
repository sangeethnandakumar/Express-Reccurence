using System;

namespace Express.Recurrence.Lib
{
    #region Class-specific Enums
    /// <summary>
    /// The Regeneration type. Is this on a specific day of the month, a custom date, or after the occurrence is completed.
    /// </summary>
    public enum YearlyRegenType { NotSet = -1, OnSpecificDayOfYear, OnCustomDateFormat, AfterOccurrenceCompleted };
    /// <summary>
    /// First part of a custom date. This would be First, Second, etc. item of the month.
    /// </summary>
    public enum YearlySpecificDatePartOne { NotSet = -1, First, Second, Third, Fourth, Last };
    /// <summary>
    /// Second part of a custom date. This is day of week, weekend day, etc.
    /// </summary>
    public enum YearlySpecificDatePartTwo { NotSet = -1, Day, Weekday, WeekendDay, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };
    /// <summary>
    /// Third part of a custom date. This is the Month of the year for which the custom date confines to..
    /// The value of this enum matches the ordinal position of the given month. So Jan = 1, Feb = 2, etc.
    /// </summary>
    public enum YearlySpecificDatePartThree { NotSet = -1, January = 1, February, March, April, May, June, July, August, September, October, November, December };
#endregion //Class-specific Enums

    public class YearlyRecurrenceSettings : RecurrenceSettings
    {
        #region Constructors
        /// <summary>
        /// Get dates by Start date only. This is for no ending date values.
        /// </summary>
        /// <param name="startDate"></param>
        public YearlyRecurrenceSettings(DateTime startDate): base(startDate){}
        /// <summary>
        /// Get dates by Start and End date boundries.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public YearlyRecurrenceSettings(DateTime startDate, DateTime endDate) : base(startDate, endDate) { }
        /// <summary>
        /// Get dates by Start date and number of occurrences.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="numberOfOccurrences"></param>
        public YearlyRecurrenceSettings(DateTime startDate, int numberOfOccurrences) : base(startDate, numberOfOccurrences) { }
        #endregion
        
        #region Private Fields
        int regenerateOnSpecificDateDayValue;
        int regenerateOnSpecificDateMonthValue;
        int adjustmentValue;
        bool getNextDateValue;
        DateTime nextDateValue;
        YearlyRegenType regenType = YearlyRegenType.NotSet;
        YearlySpecificDatePartOne specificDatePartOne = YearlySpecificDatePartOne.NotSet;
        YearlySpecificDatePartTwo specificDatePartTwo = YearlySpecificDatePartTwo.NotSet;
        YearlySpecificDatePartThree specificDatePartThree = YearlySpecificDatePartThree.NotSet;
        #endregion

        #region Public GetValues
        /// <summary>
        /// Get dates for a specific day and month of the year.
        /// </summary>
        /// <param name="specificDateDayValue">Day of the month.</param>
        /// <param name="specificDateMonthValue">Month of the year.</param>
        /// <returns></returns>
        public RecurrenceValues GetValues(int specificDateDayValue, int specificDateMonthValue)
        {
            regenerateOnSpecificDateDayValue = specificDateDayValue;
            regenerateOnSpecificDateMonthValue = specificDateMonthValue;
            regenType = YearlyRegenType.OnSpecificDayOfYear;
            return GetValues();
        }

        /// <summary>
        /// Get dates for a custom formatted date such as First weekend day of July.
        /// </summary>
        /// <param name="customDateFirstPart"></param>
        /// <param name="customDateSecondPart"></param>
        /// <param name="customDateThirdPart"></param>
        /// <returns></returns>
        public RecurrenceValues GetValues(YearlySpecificDatePartOne customDateFirstPart,YearlySpecificDatePartTwo customDateSecondPart, YearlySpecificDatePartThree customDateThirdPart)
        {
            specificDatePartOne = customDateFirstPart;
            specificDatePartTwo = customDateSecondPart;
            specificDatePartThree = customDateThirdPart;
            regenType = YearlyRegenType.OnCustomDateFormat;
            return GetValues();
        }
#endregion //Public GetValues

        #region Internal GetValues
        /// <summary>
        ///     Final overloaded function that gets the Recurrence Values. 
        ///     This is called from the RecurrenceHelper staic methods only.
        /// </summary>
        /// <returns>
        ///     A RecurrenceGenerator.RecurrenceValues value...
        /// </returns>
        internal override RecurrenceValues GetValues()
        {
            return GetRecurrenceValues();
        }
#endregion //Internal GetValues

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

            // Exit if not a Yearly seriesInfo type
            if (!seriesInfo.StartsWith("Y"))
                return null;

            info.SetSeriesInfo(seriesInfo);
            info.SetRecurrenceType(RecurrenceType.Yearly);
            // FORMATTING DEFINITIONS
            //  Y = Yearly Designator
            //  |       0 = Start Date (8 chars)
            //  |       |        1 = End Date (8 chars)
            //  |       |        |        2 = Number of occurrences (4 chars)
            //  |       |        |        |      3 = Regeneration Type (1 char)
            //  |       |        |        |      |    4 = End date type (1 char)
            //  |       |        |        |      |    |      5 = Regenerate on specific date DAY value (2 chars)
            //  |       |        |        |      |    |      |       6 = Regenerate on specific date MONTH value (2 chars)
            //  |       |        |        |      |    |      |       |       7 = Custom Date Part One (1 char)
            //  |       |        |        |      |    |      |       |       |       8 = Custom Date Part Two (1 char)
            //  |       |        |        |      |    |      |       |       |       |       9 = Custom Date Part Three (1 char)
            //  |       |        |        |      |    |      |       |       |       |       |       10 = Adjustment Value (3 chars)
            //  |       |        |        |      |    |      |       |       |       |       |       |
            // [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-24] [25-26]  [27]    [28]    [29] [31-33]
            //  Y   20071231  20171231   0000    1     1     00      00      A      A        A      000
            string startDate = seriesInfo.Substring(1, 8);

            DateTime dtStartDate = new DateTime(int.Parse(startDate.Substring(0, 4)), int.Parse(startDate.Substring(4, 2)), int.Parse(startDate.Substring(6, 2)));
            string endDate = seriesInfo.Substring(9, 8);

            string occurrences = seriesInfo.Substring(17, 4);
            string yearlyRegenType = seriesInfo.Substring(21, 1);
            string endDateType = seriesInfo.Substring(22, 1);
            string regenOnSpecificDateDayValue = seriesInfo.Substring(23, 2);
            string regenOnSpecificDateMonthValue = seriesInfo.Substring(25, 2);
            string specDatePartOne = seriesInfo.Substring(27, 1);
            string specDatePartTwo = seriesInfo.Substring(28, 1);
            string specDatePartThree = seriesInfo.Substring(29, 1);
            string adjustValue = seriesInfo.Substring(30, 3);
            endType = (EndDateType)(int.Parse(endDateType));
            noOccurrences = int.Parse(occurrences);
            YearlySpecificDatePartOne partOne = YearlySpecificDatePartOne.NotSet;
            YearlySpecificDatePartTwo partTwo = YearlySpecificDatePartTwo.NotSet;
            YearlySpecificDatePartThree partThree = YearlySpecificDatePartThree.NotSet;

            if (specDatePartOne == "Z")
                partOne = YearlySpecificDatePartOne.NotSet;
            else
                partOne = (YearlySpecificDatePartOne)(Convert.ToInt32(specDatePartOne[0]) - 65);

            if (specDatePartTwo == "Z")
                partTwo = YearlySpecificDatePartTwo.NotSet;
            else
                partTwo = (YearlySpecificDatePartTwo)(Convert.ToInt32(specDatePartTwo[0]) - 65);

            if (specDatePartThree == "Z")
                partThree = YearlySpecificDatePartThree.NotSet;
            else
                partThree = (YearlySpecificDatePartThree)(Convert.ToInt32(specDatePartThree[0]) - 64);

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

            info.SetYearlyRegenType((YearlyRegenType)(int.Parse(yearlyRegenType)));
            // Determine the Type of dates to get, specific, custom, etc.
            switch (info.YearlyRegenType)
            {
                case YearlyRegenType.OnSpecificDayOfYear:
                    info.SetSpecificDateDayValue(int.Parse(regenOnSpecificDateDayValue));
                    info.SetSpecificDateMonthValue(int.Parse(regenOnSpecificDateMonthValue));
                    break;

                case YearlyRegenType.OnCustomDateFormat:
                    info.SetYearlySpecificDatePartOne(partOne);
                    info.SetYearlySpecificDatePartTwo(partTwo);
                    info.SetYearlySpecificDatePartThree(partThree);
                    break;

                case YearlyRegenType.AfterOccurrenceCompleted:

                    break;
            }
            
            return info;
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
                case YearlyRegenType.OnSpecificDayOfYear:
                    values = GetSpecificDayOfYearDates();
                    break;

                case YearlyRegenType.OnCustomDateFormat:
                    values = GetCustomDayOfYearDates();
                    break;

                case YearlyRegenType.AfterOccurrenceCompleted:

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
        #region Internal Procedures
        internal static string GetPatternDefinition(string seriesInfo)
        {
            string returnValue = string.Empty;
            returnValue =
            " YEARLY FORMATTING DEFINITIONS\r\n" +
            "  Y = Yearly Designator\r\n" +
            "  |       0 = Start Date (8 chars)\r\n" +
            "  |       |        1 = End Date (8 chars)\r\n" +
            "  |       |        |        2 = Number of occurrences (4 chars)\r\n" +
            "  |       |        |        |      3 = Regeneration Type (1 char)\r\n" +
            "  |       |        |        |      |    4 = End date type (1 char)\r\n" +
            "  |       |        |        |      |    |      5 = Regenerate on specific date DAY value (2 chars)\r\n" +
            "  |       |        |        |      |    |      |       6 = Regenerate on specific date MONTH value (2 chars)\r\n" +
            "  |       |        |        |      |    |      |       |       7 = Custom Date Part One (1 char)\r\n" +
            "  |       |        |        |      |    |      |       |       |       8 = Custom Date Part Two (1 char)\r\n" +
            "  |       |        |        |      |    |      |       |       |       |       9 = Custom Date Part Three (1 char)\r\n" +
            "  |       |        |        |      |    |      |       |       |       |       |       10 = Adjustment Value (3 chars)\r\n" +
            "  |       |        |        |      |    |      |       |       |       |       |       |\r\n" +
            " [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-24] [25-26]  [27]    [28]    [29] [31-33]\r\n" +
            string.Format("  Y   {0}  {1}   {2}    {3}    {4}      {5}      {6}      {7}      {8}        {9}      {10}\r\n", seriesInfo.Substring(1, 8), seriesInfo.Substring(9, 8), seriesInfo.Substring(17, 4), seriesInfo.Substring(21, 1), seriesInfo.Substring(22, 1), seriesInfo.Substring(23, 2), seriesInfo.Substring(25, 2), seriesInfo.Substring(27, 1), seriesInfo.Substring(28, 1), seriesInfo.Substring(29, 1), seriesInfo.Substring(30, 3)) +
            "\r\n\r\n" +
            " Legend:\r\n\r\n" +
            " #3 - Regeneration Type\r\n" +
            "     0 = On a Specific Day of the Year\r\n" +
            "     1 = On Custom Date Format\r\n" +
            "     2 = After Occurrence Completed\r\n\r\n" +
            " #4 - End Date Type\r\n" +
            "     0 = No End Date\r\n" +
            "     1 = On a Specific Date\r\n" +
            "     2 = After a Specific Number of occurrences\r\n";
            return returnValue;

        }

        void SetValues(int specificDateDayValue, int specificDateMonthValue)
        {
            regenerateOnSpecificDateDayValue = specificDateDayValue;
            regenerateOnSpecificDateMonthValue = specificDateMonthValue;
            regenType = YearlyRegenType.OnSpecificDayOfYear;
        }

        void  SetValues(YearlySpecificDatePartOne customDateFirstPart, YearlySpecificDatePartTwo customDateSecondPart, YearlySpecificDatePartThree customDateThirdPart)
        {
            specificDatePartOne = customDateFirstPart;
            specificDatePartTwo = customDateSecondPart;
            specificDatePartThree = customDateThirdPart;
            regenType = YearlyRegenType.OnCustomDateFormat;
        }

        internal override DateTime GetNextDate(DateTime currentDate)
        {
            getNextDateValue = true;
            nextDateValue = currentDate;
            // Now get the values and return the last date found.
            RecurrenceValues values = GetValues();
            return values.EndDate;
        }

        internal static YearlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo)
        {
            return GetRecurrenceSettings(seriesInfo, -1, DateTime.MinValue);
        }
        internal static YearlyRecurrenceSettings GetRecurrenceSettings(DateTime modifiedStartDate, string seriesInfo)
        {
            return GetRecurrenceSettings(seriesInfo, -1,modifiedStartDate, DateTime.MinValue);
        }


        internal static YearlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue)
        {
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, DateTime.MinValue);
        }
        internal static YearlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedStartDate, int modifiedOccurrencesValue)
        {
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, modifiedStartDate, DateTime.MinValue);
        }


        internal static YearlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedEndDate)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedEndDate);
        }
        internal static YearlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedStartDate, DateTime modifiedEndDate)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedStartDate, modifiedEndDate);
        }


        static YearlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue, DateTime modifiedStartDate, DateTime modifiedEndDate)
        {
            // Get the Recurrence Info object. This makes it easy to work with existing series of date patterns.
            RecurrenceInfo info = YearlyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);
            YearlyRecurrenceSettings settings = null;

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
                    settings = new YearlyRecurrenceSettings(modifiedStartDate, info.NumberOfOccurrences);
                    break;

                case EndDateType.SpecificDate:
                    settings = new YearlyRecurrenceSettings(modifiedStartDate, info.EndDate.Value);
                    break;

                case EndDateType.NoEndDate:
                    settings = new YearlyRecurrenceSettings(modifiedStartDate);
                    break;
            }

            settings.AdjustmentValue = info.AdjustmentValue;

            // Determine the Type of dates to get, specific, custom, etc.
            switch (info.YearlyRegenType)
            {
                case YearlyRegenType.OnSpecificDayOfYear:
                    settings.SetValues(info.SpecificDateDayValue, info.SpecificDateMonthValue);
                    break;

                case YearlyRegenType.OnCustomDateFormat:
                    settings.SetValues(info.YearlySpecificDatePartOne, info.YearlySpecificDatePartTwo, info.YearlySpecificDatePartThree);
                    break;

                case YearlyRegenType.AfterOccurrenceCompleted:

                    break;
            }
            return settings;

        }
        static YearlyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue, DateTime modifiedEndDate)
        {
            // Get the Recurrence Info object. This makes it easy to work with existing series of date patterns.
            RecurrenceInfo info = YearlyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, info.StartDate, modifiedEndDate);
        }
#endregion //Internal Procedures

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
            string yearlyRegenType = ((int)regenType).ToString("0");
            string endDateType = ((int)base.TypeOfEndDate).ToString("0");
            string regenOnSpecificDateDayValue = regenerateOnSpecificDateDayValue.ToString("00");
            string regenOnSpecificDateMonthValue = regenerateOnSpecificDateMonthValue.ToString("00");
            string specDatePartOne = "Z";
            string specDatePartTwo = "Z";
            string specDatePartThree = "Z";

            if (specificDatePartOne != YearlySpecificDatePartOne.NotSet)
                specDatePartOne = Convert.ToString((char)(int)(specificDatePartOne + 65));
            
            if (specificDatePartTwo != YearlySpecificDatePartTwo.NotSet)
                specDatePartTwo = Convert.ToString((char)(int)(specificDatePartTwo + 65));

            if (specificDatePartThree != YearlySpecificDatePartThree.NotSet)
                specDatePartThree = Convert.ToString((char)(int)(specificDatePartThree + 64));

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
            //  |       |        |        |      |    |      |       6 = Regenerate on specific date MONTH value (2 chars)
            //  |       |        |        |      |    |      |       |       7 = Custom Date Part One (1 char)
            //  |       |        |        |      |    |      |       |       |       8 = Custom Date Part Two (1 char)
            //  |       |        |        |      |    |      |       |       |       |       9 = Custom Date Part Three (1 char)
            //  |       |        |        |      |    |      |       |       |       |       |       10 = Adjustment Value (3 chars)
            //  |       |        |        |      |    |      |       |       |       |       |       |
            // [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-24] [25-26]  [27]    [28]    [29] [31-33]
            //  Y   20071237  20171231   0000    1     1     00      00      A      A        A      000
            info = string.Format("Y{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", base.StartDate.ToString("yyyyMMdd"), endDate, occurrences, yearlyRegenType, endDateType, regenOnSpecificDateDayValue, regenOnSpecificDateMonthValue, specDatePartOne, specDatePartTwo, specDatePartThree, adjustValue);
            return info;
        }

        /// <summary>
        /// Get recurring dates from a specific constant date such as 27 July.
        /// </summary>
        /// <returns></returns>
        RecurrenceValues GetSpecificDayOfYearDates()
        {
            RecurrenceValues values = new RecurrenceValues();
            DateTime dt = base.StartDate;
            int dayValue = regenerateOnSpecificDateDayValue;
            int daysOfMonth = DateTime.DaysInMonth(dt.Year, regenerateOnSpecificDateMonthValue);
            // Get the max days of the month and make sure it's not 
            // less than the specified day value trying to be set.
            if (daysOfMonth < regenerateOnSpecificDateDayValue)
                dayValue = daysOfMonth;

            // Determine if start date is greater than the Day and Month values
            // for a specific date.
            DateTime newDate = new DateTime(dt.Year, regenerateOnSpecificDateMonthValue, dayValue);
            // Is the specific date before the start date, if so 
            // then make the specific date next year.
            if (newDate < dt)
                dt = newDate.AddYears(1);
            else
                dt = newDate;

            if (getNextDateValue)
            {
                do
                {
                    values.AddDateValue(dt, adjustmentValue);
                    if (values.Values[values.Values.Count - 1] > nextDateValue)
                        break;
                    dt = dt.AddYears(1);
                    dt = GetCorrectedDate(dt);
                } while (dt <= nextDateValue.AddYears(1)); // Ensure the last date if greater than what's needed for the next date in the cycle
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
                            values.AddDateValue(GetCorrectedDate(dt.AddYears(i)), adjustmentValue);
                        }
                        break;

                    case EndDateType.SpecificDate:
                        do
                        {
                            values.AddDateValue(dt, adjustmentValue);
                            dt = dt.AddYears(1);
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
        RecurrenceValues GetCustomDayOfYearDates()
        {
            if (this.SpecificDatePartOne == YearlySpecificDatePartOne.NotSet)
                throw new Exception("The First part of the custom date has not been set.");
            if (this.SpecificDatePartTwo == YearlySpecificDatePartTwo.NotSet)
                throw new Exception("The Second part of the custom date has not been set.");
            if (this.SpecificDatePartThree == YearlySpecificDatePartThree.NotSet)
                throw new Exception("The Third part of the custom date has not been set.");

            RecurrenceValues values = new RecurrenceValues();
            DateTime dt = base.StartDate;
            int year = dt.Year;

            if (getNextDateValue)
            {
                do
                {
                    dt = GetCustomDate(year);
                    // If the date returned is less than the start date
                    // then do it again to increment past the start date
                    if (dt < base.StartDate)
                    {
                        year++;
                        dt = GetCustomDate(year);
                    }
                    year++;
                    values.AddDateValue(dt, adjustmentValue);
                    if (values.Values[values.Values.Count - 1] > nextDateValue)
                        break;
                } while (dt <= nextDateValue.AddYears(1)); // Ensure the last date if greater than what's needed for the next date in the cycle
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
                            dt = GetCustomDate(year);
                            // If the date returned is less than the start date
                            // then do it again to increment past the start date
                            if (dt < base.StartDate)
                            {
                                year++;
                                dt = GetCustomDate(year);
                            }
                            year++;
                            values.AddDateValue(dt, adjustmentValue);
                        }
                        break;

                    case EndDateType.SpecificDate:
                        do
                        {
                            dt = GetCustomDate(year);
                            // If the date returned is less than the start date
                            // then do it again to increment past the start date
                            if (dt < base.StartDate)
                            {
                                year++;
                                dt = GetCustomDate(year);
                            }
                            year++;
                            values.AddDateValue(dt, adjustmentValue);
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
        DateTime GetCustomDate(int year)
        {
            DateTime dt = new DateTime(year, (int)SpecificDatePartThree, 1);
            int day = 1;
            int firstPart = (int)SpecificDatePartOne + 1;
            int daysOfMonth = DateTime.DaysInMonth(year, dt.Month);

            switch (SpecificDatePartTwo)
            {
                case YearlySpecificDatePartTwo.Day:
                    // If only getting the Last day of the month
                    if (SpecificDatePartOne == YearlySpecificDatePartOne.Last)
                        dt = new DateTime(year, dt.Month, DateTime.DaysInMonth(year, dt.Month));
                    else
                        // Get a specific day of the month such as First, Second, Third, Fourth
                        dt = new DateTime(year, dt.Month, firstPart);
                    
                    break;

                case YearlySpecificDatePartTwo.Weekday:
                    int weekDayCount = 0;
                    DateTime lastWeekday = dt;
                    do
                    {
                        // Check for anything other than Saturday and Sunday
                        if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                        {
                            // Get a specific Weekday of the Month
                            if (SpecificDatePartOne != YearlySpecificDatePartOne.Last)
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
                    if (SpecificDatePartOne == YearlySpecificDatePartOne.Last)
                        dt = lastWeekday;

                    break;

                case YearlySpecificDatePartTwo.WeekendDay:
                    int weekendDayCount = 0;
                    DateTime lastWeekendday = dt;
                    do
                    {
                        // Check for anything other than Saturday and Sunday
                        if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                        {
                            // Get a specific Weekday of the Month
                            if (SpecificDatePartOne != YearlySpecificDatePartOne.Last)
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
                    if (SpecificDatePartOne == YearlySpecificDatePartOne.Last)
                        dt = lastWeekendday;

                    break;
                    
                case YearlySpecificDatePartTwo.Monday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Monday, daysOfMonth, firstPart);
                    break;

                case YearlySpecificDatePartTwo.Tuesday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Tuesday, daysOfMonth, firstPart);
                    break;

                case YearlySpecificDatePartTwo.Wednesday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Wednesday, daysOfMonth, firstPart);
                    break;

                case YearlySpecificDatePartTwo.Thursday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Thursday, daysOfMonth, firstPart);
                    break;
                    
                case YearlySpecificDatePartTwo.Friday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Friday, daysOfMonth, firstPart);
                    break;

                case YearlySpecificDatePartTwo.Saturday:
                    dt = GetCustomWeekday(dt, DayOfWeek.Saturday, daysOfMonth, firstPart);
                    break;

                case YearlySpecificDatePartTwo.Sunday:
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
                    if (SpecificDatePartOne != YearlySpecificDatePartOne.Last)
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
            if (SpecificDatePartOne == YearlySpecificDatePartOne.Last)
                returnDate = lastDOW;

            return returnDate;
        }
        int GetDatePartOneValue()
        {
            int val = 0;
            switch (SpecificDatePartOne)
            {
                case YearlySpecificDatePartOne.First:
                    val = 1;
                    break;
                case YearlySpecificDatePartOne.Second:
                    val = 2;
                    break;
                case YearlySpecificDatePartOne.Third:
                    val = 3;
                    break;
                case YearlySpecificDatePartOne.Fourth:
                    val = 4;
                    break;
            }
            return val;
        }
#endregion //Private Procedures

        #region Public Fields
        /// <summary>
        /// Part of the Custom date that equates to the month of year.
        /// </summary>
        public YearlySpecificDatePartThree SpecificDatePartThree
        {
            get
            {
                return specificDatePartThree;
            }
        }

        /// <summary>
        /// Part of the Custom date that is the day part of the month such as weekend day, Tuesday, Wednesday, weekday, etc.
        /// </summary>
        public YearlySpecificDatePartTwo SpecificDatePartTwo
        {
            get
            {
                return specificDatePartTwo;
            }
        }

        /// <summary>
        /// Part of the Custom date that is the part to get such as First, Second, Last, etc.
        /// </summary>
        public YearlySpecificDatePartOne SpecificDatePartOne
        {
            get
            {
                return specificDatePartOne;
            }
        }

        /// <summary>
        /// Regeneration type such as by Specific day of the year, Custom date, etc.
        /// </summary>
        public YearlyRegenType RegenType
        {
            get
            {
                return regenType;
            }
        }

        /// <summary>
        /// What day of the month do you want to regenerate dates when by a specific day of the year.
        /// </summary>
        public int RegenerateOnSpecificDateDayValue
        {
            get
            {
                return regenerateOnSpecificDateDayValue;
            }
        }

        /// <summary>
        /// What month of the year do you want to regenerate dates when by a specific day of the year.
        /// </summary>
        public int RegenerateOnSpecificDateMonthValue
        {
            get
            {
                return regenerateOnSpecificDateMonthValue;
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
#endregion //Public Fields
        
    }
}
