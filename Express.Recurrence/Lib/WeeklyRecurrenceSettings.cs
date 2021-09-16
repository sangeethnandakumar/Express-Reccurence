using System;

namespace Express.Recurrence.Lib
{
    #region Class-specific Enums
    /// <summary>
    /// The Regeneration type. 
    /// </summary>
    public enum WeeklyRegenType { NotSet = -1, OnEveryXWeeks};
#endregion //Class-specific Enums

    #region Class-specific Structs
    public struct SelectedDayOfWeekValues
    {
        public bool Sunday;
        public bool Monday;
        public bool Tuesday;
        public bool Wednesday;
        public bool Thursday;
        public bool Friday;
        public bool Saturday;
    }
#endregion //Class-specific Structs

    public class WeeklyRecurrenceSettings : RecurrenceSettings
    {
        #region Constructors
        /// <summary>
        /// Get dates by Start date only. This is for no ending date values.
        /// </summary>
        /// <param name="startDate"></param>
        public WeeklyRecurrenceSettings(DateTime startDate) : base(startDate) { }
        /// <summary>
        /// Get dates by Start and End date boundries.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public WeeklyRecurrenceSettings(DateTime startDate, DateTime endDate) : base(startDate, endDate) { }
        /// <summary>
        /// Get dates by Start date and number of occurrences.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="numberOfOccurrences"></param>
        public WeeklyRecurrenceSettings(DateTime startDate, int numberOfOccurrences) : base(startDate, numberOfOccurrences) { }
        #endregion

        #region Private Fields
        
        WeeklyRegenType regenType = WeeklyRegenType.OnEveryXWeeks;
        SelectedDayOfWeekValues selectedDays;
        int regenEveryXWeeks;
        bool getNextDateValue;
        DateTime nextDateValue;
#endregion //Private Fields

        #region Public GetValues
        /// <summary>
        /// Get day values. This overload is for every x-weeks.
        /// </summary>
        /// <param name="regenEveryXDays">Interval of weeks. Every x-weeks.</param>
        /// <returns>RecurrenceValues</returns>
        public RecurrenceValues GetValues(int regenEveryXWeeks, SelectedDayOfWeekValues selectedDays)
        {
            this.regenEveryXWeeks = regenEveryXWeeks;
            regenType = WeeklyRegenType.OnEveryXWeeks;
            this.selectedDays = selectedDays;
            return GetValues();
        }
#endregion //Public GetValues

        internal static RecurrenceInfo GetFriendlyRecurrenceInfo(string seriesInfo)
        {
            RecurrenceInfo info = new RecurrenceInfo();
            EndDateType endType = EndDateType.NotDefined;
            DateTime endDateValue = DateTime.MinValue;
            int noOccurrences;

            // Exit if not a Weekly seriesInfo type
            if (!seriesInfo.StartsWith("W"))
                return null;

            info.SetSeriesInfo(seriesInfo);
            info.SetRecurrenceType(RecurrenceType.Weekly);
            
            // FORMATTING DEFINITIONS
            //  Y = Yearly Designator
            //  |       0 = Start Date (8 chars)
            //  |       |        1 = End Date (8 chars)
            //  |       |        |        2 = Number of occurrences (4 chars)
            //  |       |        |        |      3 = Regeneration Type (1 char)
            //  |       |        |        |      |    4 = End date type (1 char)
            //  |       |        |        |      |    |      5 = Regenerate on Sunday
            //  |       |        |        |      |    |      |       6 = Regenerate on Monday
            //  |       |        |        |      |    |      |       |       7 = Regenerate on Tuesday
            //  |       |        |        |      |    |      |       |       |       8 = Regenerate on Wednesday
            //  |       |        |        |      |    |      |       |       |       |      9 = Regenerate on Thursday
            //  |       |        |        |      |    |      |       |       |       |      |      10 = Regenerate on Friday
            //  |       |        |        |      |    |      |       |       |       |      |       |      11 = Regenerate on Saturday
            //  |       |        |        |      |    |      |       |       |       |      |       |      |        12  Regen Every x weeks
            //  |       |        |        |      |    |      |       |       |       |      |       |      |         |
            // [0]    [1-8]    [9-16]  [17-20]  [21] [22]  [23]    [24]    [25]    [26]   [27]     [28]   [29]      [30-32]
            //  W   20071231  20171231   0000    1     1     T       T      F        F      F       F      F        000 
            string startDate = seriesInfo.Substring(1, 8);

            DateTime dtStartDate = new DateTime(int.Parse(startDate.Substring(0, 4)), int.Parse(startDate.Substring(4, 2)), int.Parse(startDate.Substring(6, 2)));
            string endDate = seriesInfo.Substring(9, 8);
            SelectedDayOfWeekValues selectedDays = new SelectedDayOfWeekValues();
            string occurrences = seriesInfo.Substring(17, 4);
            string weeklyRegenType = seriesInfo.Substring(21, 1);
            string endDateType = seriesInfo.Substring(22, 1);
            int regenXWeeks = int.Parse(seriesInfo.Substring(30, 3));

            endType = (EndDateType)(int.Parse(endDateType));
            noOccurrences = int.Parse(occurrences);

            selectedDays.Sunday = (seriesInfo.Substring(23, 1) == "Y") ? true : false;
            selectedDays.Monday = (seriesInfo.Substring(24, 1) == "Y") ? true : false;
            selectedDays.Tuesday = (seriesInfo.Substring(25, 1) == "Y") ? true : false;
            selectedDays.Wednesday = (seriesInfo.Substring(26, 1) == "Y") ? true : false;
            selectedDays.Thursday = (seriesInfo.Substring(27, 1) == "Y") ? true : false;
            selectedDays.Friday = (seriesInfo.Substring(28, 1) == "Y") ? true : false;
            selectedDays.Saturday = (seriesInfo.Substring(29, 1) == "Y") ? true : false;

            endType = (EndDateType)(int.Parse(endDateType));
            noOccurrences = int.Parse(occurrences);

            // Get the EndDate before any modifications on it are performed
            if (endType == EndDateType.SpecificDate)
                endDateValue = new DateTime(int.Parse(endDate.Substring(0, 4)), int.Parse(endDate.Substring(4, 2)), int.Parse(endDate.Substring(6, 2)));

            info.SetEndDateType(endType);
            // Determine the Constructor by the type of End Date.
            // All constructors start with a Start date at a minimum.
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

            info.SetWeeklyRegenType((WeeklyRegenType)(int.Parse(weeklyRegenType)));
            // Determine the Type of dates to get, specific, custom, etc.
            switch (info.WeeklyRegenType)
            {
                case WeeklyRegenType.OnEveryXWeeks:
                    info.SetSelectedDayOfWeekValues(selectedDays);
                    info.SetRegenEveryXWeeks(regenXWeeks);
                    break;

                case WeeklyRegenType.NotSet:

                    break;
            }

            return info;


        }


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
                case WeeklyRegenType.OnEveryXWeeks:
                    values = GetEveryXWeeksValues();
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
            " WEEKLY FORMATTING DEFINITIONS \r\n" +
            "  Y = Yearly Designator \r\n" +
            "  |       0 = Start Date (8 chars) \r\n" +
            "  |       |        1 = End Date (8 chars) \r\n" +
            "  |       |        |        2 = Number of occurrences (4 chars) \r\n" +
            "  |       |        |        |      3 = Regeneration Type (1 char) \r\n" +
            "  |       |        |        |      |    4 = End date type (1 char) \r\n" +
            "  |       |        |        |      |    |      5 = Regenerate on Sunday \r\n" +
            "  |       |        |        |      |    |      |       6 = Regenerate on Monday \r\n" +
            "  |       |        |        |      |    |      |       |       7 = Regenerate on Tuesday \r\n" +
            "  |       |        |        |      |    |      |       |       |       8 = Regenerate on Wednesday \r\n" +
            "  |       |        |        |      |    |      |       |       |       |      9 = Regenerate on Thursday \r\n" +
            "  |       |        |        |      |    |      |       |       |       |      |      10 = Regenerate on Friday \r\n" +
            "  |       |        |        |      |    |      |       |       |       |      |       |      11 = Regenerate on Saturday \r\n" +
            "  |       |        |        |      |    |      |       |       |       |      |       |      |        12  Regen Every x weeks \r\n" +
            "  |       |        |        |      |    |      |       |       |       |      |       |      |         | \r\n" +
            " [0]    [1-8]    [9-16]  [17-20]  [21] [22]  [23]    [24]    [25]    [26]   [27]     [28]   [29]      [30-32] \r\n" +
            string.Format("  W   {0}  {1}   {2}    {3}     {4}     {5}       {6}       {7}      {8}       {9}       {10}      {11}         {12}\r\n", seriesInfo.Substring(1, 8), seriesInfo.Substring(9, 8), seriesInfo.Substring(17, 4), seriesInfo.Substring(21, 1), seriesInfo.Substring(22, 1), seriesInfo.Substring(23, 1), seriesInfo.Substring(24, 1), seriesInfo.Substring(25, 1), seriesInfo.Substring(26, 1), seriesInfo.Substring(27, 1), seriesInfo.Substring(28, 1), seriesInfo.Substring(29, 1), seriesInfo.Substring(30, 3));
            return returnValue;
        }
        /// <summary>
        /// Set the values in preperation for getting the Next date in the series.
        /// </summary>
        /// <param name="regenEveryXWeeks">Value to regenerate the dates every x-weeks</param>
        /// <param name="selectedDays">Struct of days selected for the week.</param>
        void SetValues(int regenEveryXWeeks, SelectedDayOfWeekValues selectedDays)
        {
            this.regenEveryXWeeks = regenEveryXWeeks;
            regenType = WeeklyRegenType.OnEveryXWeeks;
            this.selectedDays = selectedDays;
        }

        /// <summary>
        ///     Get the next date
        /// </summary>
        /// <param name="currentDate" type="System.DateTime">
        ///     <para>
        ///         
        ///     </para>
        /// </param>
        /// <returns>
        ///     A System.DateTime value...
        /// </returns>
        internal override DateTime GetNextDate(DateTime currentDate)
        {
            getNextDateValue = true;
            nextDateValue = currentDate;
            // Now get the values and return the last date found.
            RecurrenceValues values = GetValues();
            return values.EndDate;
        }

        internal static WeeklyRecurrenceSettings GetRecurrenceSettings(string seriesInfo)
        {
            return GetRecurrenceSettings(seriesInfo, -1, DateTime.MinValue);
        }
        internal static WeeklyRecurrenceSettings GetRecurrenceSettings(DateTime modifiedStartDate, string seriesInfo)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedStartDate, DateTime.MinValue);
        }

        internal static WeeklyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue)
        {
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, DateTime.MinValue);
        }
        internal static WeeklyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedStartDate, int modifiedOccurrencesValue)
        {
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, modifiedStartDate, DateTime.MinValue);
        }

        internal static WeeklyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedEndDate)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedEndDate);
        }
        internal static WeeklyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedStartDate, DateTime modifiedEndDate)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedStartDate, modifiedEndDate);
        }

        static WeeklyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue, DateTime modifiedStartDate, DateTime modifiedEndDate)
        {
            WeeklyRecurrenceSettings settings = null;
            RecurrenceInfo info = WeeklyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);

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
                    settings = new WeeklyRecurrenceSettings(modifiedStartDate, info.NumberOfOccurrences);
                    break;

                case EndDateType.SpecificDate:
                    settings = new WeeklyRecurrenceSettings(modifiedStartDate, info.EndDate.Value);
                    break;

                case EndDateType.NoEndDate:
                    settings = new WeeklyRecurrenceSettings(modifiedStartDate);
                    break;
            }


            // Determine the Type of dates to get, specific, custom, etc.
            switch (info.WeeklyRegenType)
            {
                case WeeklyRegenType.OnEveryXWeeks:
                    settings.SetValues(info.WeeklyRegenEveryXWeeks, info.WeeklySelectedDays);
                    break;

                case WeeklyRegenType.NotSet:

                    break;
            }

            return settings;

        }

        static WeeklyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue,DateTime modifiedEndDate)
        {
            RecurrenceInfo info = WeeklyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);

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
            string info = string.Empty;
            string endDate = "ZZZZZZZZ"; // Default for no ending date.
            string occurrences = base.NumberOfOccurrences.ToString("0000");
            string weeklyRegenType = ((int)regenType).ToString("0");
            string endDateType = ((int)base.TypeOfEndDate).ToString("0");
            string regenXWeeks = regenEveryXWeeks.ToString("000");
            string daySelectedSunday = selectedDays.Sunday ? "Y" : "N";
            string daySelectedMonday = selectedDays.Monday ? "Y" : "N";
            string daySelectedTuesday = selectedDays.Tuesday ? "Y" : "N";
            string daySelectedWednesday = selectedDays.Wednesday ? "Y" : "N";
            string daySelectedThursday = selectedDays.Thursday ? "Y" : "N";
            string daySelectedFriday = selectedDays.Friday ? "Y" : "N";
            string daySelectedSaturday = selectedDays.Saturday ? "Y" : "N";

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
            //  |       |        |        |      |    |      5 = Regenerate on Sunday
            //  |       |        |        |      |    |      |       6 = Regenerate on Monday
            //  |       |        |        |      |    |      |       |       7 = Regenerate on Tuesday
            //  |       |        |        |      |    |      |       |       |       8 = Regenerate on Wednesday
            //  |       |        |        |      |    |      |       |       |       |      9 = Regenerate on Thursday
            //  |       |        |        |      |    |      |       |       |       |      |      10 = Regenerate on Friday
            //  |       |        |        |      |    |      |       |       |       |      |       |      11 = Regenerate on Saturday
            //  |       |        |        |      |    |      |       |       |       |      |       |      |        12  Regen Every x weeks
            //  |       |        |        |      |    |      |       |       |       |      |       |      |         |
            // [0]    [1-8]    [9-16]  [17-20]  [21] [22]  [23]    [24]    [25]    [26]   [27]     [28]   [29]      [30]
            //  W   20071231  01082017   0000    1     1     T       T      F        F      F       F      F        000 
            info = string.Format("W{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", base.StartDate.ToString("yyyyMMdd"), endDate, occurrences, weeklyRegenType, endDateType, daySelectedSunday, daySelectedMonday, daySelectedTuesday, daySelectedWednesday, daySelectedThursday, daySelectedFriday, daySelectedSaturday, regenXWeeks);
            return info;
        }

        RecurrenceValues GetEveryXWeeksValues()
        {
            RecurrenceValues values = new RecurrenceValues();
            DateTime dt = base.StartDate.AddDays(-1); // Backup a day so the first instance of GetNextDay will increment to the next day.

            if (getNextDateValue)
            {
                do
                {
                    dt = GetNextDay(dt);
                    values.AddDateValue(dt);
                    if (values.Values[values.Values.Count - 1] > nextDateValue)
                        break;
                } while (dt <= nextDateValue.AddDays((regenEveryXWeeks * 7) + 7)); // Ensure the last date if greater than what's needed for the next date in the cycle
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
                            dt = GetNextDay(dt);
                            values.AddDateValue(dt);
                        }
                        break;

                    case EndDateType.SpecificDate:
                        do
                        {
                            dt = GetNextDay(dt);
                            // Handle for dates past the end date
                            if (dt > base.EndDate)
                                break;

                            values.AddDateValue(dt);
                        } while (dt <= base.EndDate);
                        break;

                    default:
                        throw new ArgumentNullException("TypeOfEndDate", "The TypeOfEndDate property has not been set.");
                }
            }

            return values;
        }

        DateTime GetNextDay(DateTime input)
        {
            DateTime? returnDate = null;

            // Get the return date by incrementing the date
            // and checking the value against the selected days
            // of the week.
            do
            {
                input = input.AddDays(1);
                switch (input.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (selectedDays.Sunday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Monday:
                        if (selectedDays.Monday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Tuesday:
                        if (selectedDays.Tuesday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Wednesday:
                        if (selectedDays.Wednesday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Thursday:
                        if (selectedDays.Thursday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Friday:
                        if (selectedDays.Friday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Saturday:
                        if (selectedDays.Saturday)
                            returnDate = input;
                        else
                        {
                            // Increment by weeks if regenXWeeks has a value 
                            // greater than 1 which is default.
                            // But only increment if we've gone over
                            // at least 7 days already.
                            if (regenEveryXWeeks > 1 )
                                input = input.AddDays((regenEveryXWeeks -1) * 7);
                        }
                        break;
                }
            } while (!returnDate.HasValue);
            return returnDate.Value;
        }
#endregion //Private Procedures

        #region Public Fields
        /// <summary>
        /// What is the interval to generate dates. This is used to skip weeks in the cycle.
        /// </summary>
        public int RegenEveryXWeeks
        {
            get
            {
                return regenEveryXWeeks;
            }
        }

        public SelectedDayOfWeekValues SelectedDays
        {
            get
            {
                return selectedDays;
            }
        }

        /// <summary>
        /// What is the regeneration type such as Specific day of month, custom date, etc.
        /// </summary>
        public WeeklyRegenType RegenType
        {
            get
            {
                return regenType;
            }
        }

#endregion //Public Fields

    }
}
