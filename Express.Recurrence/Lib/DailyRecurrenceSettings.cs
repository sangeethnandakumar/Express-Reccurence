using System;

namespace Express.Recurrence.Lib
{
    #region Class-specific Enums
    /// <summary>
    /// The Regeneration type. 
    /// </summary>
    public enum DailyRegenType { NotSet = -1, OnEveryXDays, OnEveryWeekday};
#endregion //Class-specific Enums

    public class DailyRecurrenceSettings : RecurrenceSettings
    {
        #region Constructors
        /// <summary>
        /// Get dates by Start date only. This is for no ending date values.
        /// </summary>
        /// <param name="startDate"></param>
        public DailyRecurrenceSettings(DateTime startDate) : base(startDate) { }
        /// <summary>
        /// Get dates by Start and End date boundries.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public DailyRecurrenceSettings(DateTime startDate, DateTime endDate) : base(startDate, endDate) { }
        /// <summary>
        /// Get dates by Start date and number of occurrences.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="numberOfOccurrences"></param>
        public DailyRecurrenceSettings(DateTime startDate, int numberOfOccurrences) : base(startDate, numberOfOccurrences) { }
        #endregion

        #region Private Fields
        DailyRegenType regenType = DailyRegenType.OnEveryWeekday;
        int regenEveryXDays = 1;
        bool getNextDateValue;
        DateTime nextDateValue;
        DateTime finalNextDateValue;
        #endregion

        #region Public GetValues 
        /// <summary>
        /// Get day values. This overload is for every x-days.
        /// </summary>
        /// <param name="regenEveryXDays">Interval of days. Every x-days.</param>
        /// <returns>RecurrenceValues</returns>
        public RecurrenceValues GetValues(int regenEveryXDays)
        {
            this.regenEveryXDays = regenEveryXDays;
            regenType = DailyRegenType.OnEveryXDays;
            return GetValues();
        }

        /// <summary>
        ///     An overload to use to get either every weekday or just every x-days
        /// </summary>
        /// <param name="regenEveryXDays" type="int">
        ///     <para>
        ///         Interval of days. Every x-days.
        ///     </para>
        /// </param>
        /// <param name="regenType" type="BOCA.RecurrenceGenerator.DailyRegenType">
        ///     <para>
        ///         Type of regeneration to perform. Every x-days or every weekday.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A RecurrenceGenerator.RecurrenceValues value...
        /// </returns>
        public RecurrenceValues GetValues(int regenEveryXDays, DailyRegenType regenType)
        {
            this.regenEveryXDays = regenEveryXDays;
            this.regenType = regenType;
            return GetValues();
        }

#endregion //Public GetValues 

        internal static RecurrenceInfo GetFriendlyRecurrenceInfo(string seriesInfo)
        {
            RecurrenceInfo info = new RecurrenceInfo();
            EndDateType endType = EndDateType.NotDefined;
            DateTime endDateValue = DateTime.MinValue;
            int noOccurrences;

            // Exit if not a Daily seriesInfo type
            if (!seriesInfo.StartsWith("D"))
                return null;

            info.SetRecurrenceType(RecurrenceType.Daily);
            info.SetSeriesInfo(seriesInfo);
            // FORMATTING DEFINITIONS
            //  Y = Yearly Designator
            //  |       0 = Start Date (8 chars)
            //  |       |        1 = End Date (8 chars)
            //  |       |        |        2 = Number of occurrences (4 chars)
            //  |       |        |        |      3 = Regeneration Type (1 char)
            //  |       |        |        |      |    4 = End date type (1 char)
            //  |       |        |        |      |    |      5 = Regen Every x weeks
            //  |       |        |        |      |    |      |
            //  |       |        |        |      |    |      |
            // [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-25]
            //  D   20071231  20171231   0000    1     1    000
            string startDate = seriesInfo.Substring(1, 8);

            DateTime dtStartDate = new DateTime(int.Parse(startDate.Substring(0, 4)), int.Parse(startDate.Substring(4, 2)), int.Parse(startDate.Substring(6, 2)));
            string endDate = seriesInfo.Substring(9, 8);
            string occurrences = seriesInfo.Substring(17, 4);
            string dailyRegenType = seriesInfo.Substring(21, 1);
            string endDateType = seriesInfo.Substring(22, 1);
            int regenXDays = int.Parse(seriesInfo.Substring(23, 3));
            endType = (EndDateType)(int.Parse(endDateType));
            noOccurrences = int.Parse(occurrences);

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

            info.SetDailyRegenType((DailyRegenType)(int.Parse(dailyRegenType)));
            // Determine the Type of dates to get, specific, custom, etc.
            switch (info.DailyRegenType)
            {
                case DailyRegenType.OnEveryXDays:
                    info.SetDailyRegenEveryXDays(regenXDays);
                    break;

                case DailyRegenType.OnEveryWeekday:
                    // This is default. Nothing to set
                    break;

                case DailyRegenType.NotSet:

                    break;
            }

            return info;


        }


        #region Internal GetValues
        /// <summary>
        /// Get day values. Default is to get every weekday. This is called from the RecurrenceHelper staic methods only.
        /// </summary>
        /// <returns>RecurrenceValues</returns>
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
                case DailyRegenType.OnEveryXDays:
                    values = GetEveryXDaysValues();
                    break;

                case DailyRegenType.OnEveryWeekday:
                    values = GetEveryWeekday();
                    break;

            }
            // Values will be null if just getting next date in series. No need
            // to fill the RecurrenceValues collection if all we need is the last date.
            if (values != null)
            {
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
            }

            return values;

        }
#endregion //Internal GetValues

        #region Internal Procedures
        internal static string GetPatternDefinition(string seriesInfo)
        {
            string returnValue = string.Empty;
            returnValue =
            " DAILY FORMATTING DEFINITIONS\r\n" +
            "  Y = Yearly Designator\r\n" +
            "  |       0 = Start Date (8 chars)\r\n" +
            "  |       |        1 = End Date (8 chars)\r\n" +
            "  |       |        |        2 = Number of occurrences (4 chars)\r\n" +
            "  |       |        |        |      3 = Regeneration Type (1 char)\r\n" +
            "  |       |        |        |      |    4 = End date type (1 char)\r\n" +
            "  |       |        |        |      |    |      5 = Regen Every x weeks\r\n" +
            "  |       |        |        |      |    |      |\r\n" +
            "  |       |        |        |      |    |      |\r\n" +
            " [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-25]\r\n" +
            string.Format("  D   {0}  {1}   {2}    {3}     {4}     {5} \r\n", seriesInfo.Substring(1, 8), seriesInfo.Substring(9, 8), seriesInfo.Substring(17, 4), seriesInfo.Substring(21, 1), seriesInfo.Substring(22, 1), seriesInfo.Substring(23, 3));
            return returnValue;
        }
        void SetValues(int regenEveryXDays)
        {
            this.regenEveryXDays = regenEveryXDays;
            regenType = DailyRegenType.OnEveryXDays;
        }

        internal override DateTime GetNextDate(DateTime currentDate)
        {
            getNextDateValue = true;
            nextDateValue = currentDate;
            // Run the date processing to set the last date value.
            GetValues();
            return finalNextDateValue;
        }

        internal static DailyRecurrenceSettings GetRecurrenceSettings(string seriesInfo)
        {
            return GetRecurrenceSettings(seriesInfo, -1, DateTime.MinValue);
        }
        internal static DailyRecurrenceSettings GetRecurrenceSettings(DateTime modifiedStartDate, string seriesInfo)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedStartDate, DateTime.MinValue);
        }

        internal static DailyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue)
        {
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, DateTime.MinValue);
        }
        internal static DailyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedStartDate, int modifiedOccurrencesValue)
        {
            return GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue, modifiedStartDate, DateTime.MinValue);
        }


        internal static DailyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedEndDate)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedEndDate);
        }
        internal static DailyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, DateTime modifiedStartDate, DateTime modifiedEndDate)
        {
            return GetRecurrenceSettings(seriesInfo, -1, modifiedStartDate, modifiedEndDate);
        }


        static DailyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue, DateTime modifiedStartDate, DateTime modifiedEndDate)
        {
            DailyRecurrenceSettings settings = null;
            RecurrenceInfo info = DailyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);

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
                    settings = new DailyRecurrenceSettings(modifiedStartDate, info.NumberOfOccurrences);
                    break;

                case EndDateType.SpecificDate:
                    settings = new DailyRecurrenceSettings(modifiedStartDate, info.EndDate.Value);
                    break;

                case EndDateType.NoEndDate:
                    settings = new DailyRecurrenceSettings(modifiedStartDate);
                    break;
            }

            // Determine the Type of dates to get, specific, custom, etc.
            switch (info.DailyRegenType)
            {
                case DailyRegenType.OnEveryXDays:
                    settings.SetValues(info.DailyRegenEveryXDays);
                    break;

                case DailyRegenType.OnEveryWeekday:
                    // This is default. Nothing to set
                    break;

                case DailyRegenType.NotSet:

                    break;
            }

            return settings;
        }


        static DailyRecurrenceSettings GetRecurrenceSettings(string seriesInfo, int modifiedOccurrencesValue, DateTime modifiedEndDate)
        {
            RecurrenceInfo info = DailyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);

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
            string dailyRegenType = ((int)regenType).ToString("0");
            string endDateType = ((int)base.TypeOfEndDate).ToString("0");
            string regenXDays = regenEveryXDays.ToString("000");

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
            //  |       |        |        |      |    |      5 = Regen Every x weeks
            //  |       |        |        |      |    |      |
            //  |       |        |        |      |    |      |
            //  |       |        |        |      |    |      |
            //  |       |        |        |      |    |      |
            //  |       |        |        |      |    |      |
            //  |       |        |        |      |    |      |
            //  |       |        |        |      |    |      |
            //  |       |        |        |      |    |      |
            // [0]    [1-8]    [9-16]  [17-20]  [21] [22] [23-25]
            //  D   20071231  20171231   0000    1     1    000
            info = string.Format("D{0}{1}{2}{3}{4}{5}", base.StartDate.ToString("yyyyMMdd"), endDate, occurrences, dailyRegenType, endDateType, regenXDays);
            return info;
        }


        /// <summary>
        /// Get the values for just weekdays.
        /// </summary>
        /// <returns>RecurrenceValues</returns>
        RecurrenceValues GetEveryWeekday()
        {
            RecurrenceValues values;
            DateTime dt = base.StartDate;
            // Make sure the first date is a weekday
            if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                dt = GetNextWeekday(dt);

            if (getNextDateValue)
            {
                do
                {
                    finalNextDateValue = dt;
                    if (finalNextDateValue > nextDateValue)
                        break;
                    dt = GetNextWeekday(dt);
                } while (dt <= nextDateValue.AddDays(3));  // Make sure it's past what may possibly be the next weekday.
                return null;
            }
            else
            {
                values = new RecurrenceValues();
                switch (base.TypeOfEndDate)
                {
                    case EndDateType.NoEndDate:
                        throw new Exception("The ability to create recurring dates with no End date is not currently available.");

                    case EndDateType.NumberOfOccurrences:

                        for (int i = 0; i < base.NumberOfOccurrences; i++)
                        {
                            values.AddDateValue(dt);
                            dt = GetNextWeekday(dt);
                        }
                        break;

                    case EndDateType.SpecificDate:
                        do
                        {
                            values.AddDateValue(dt);
                            dt = GetNextWeekday(dt);
                        } while (dt <= base.EndDate);
                        break;

                    default:
                        throw new ArgumentNullException("TypeOfEndDate", "The TypeOfEndDate property has not been set.");
                }
                return values;
            }
        }

        /// <summary>
        /// Get the next Weekday value. This will increment the input date until it finds the next non-Saturday and non-Sunday dates.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>DateTime</returns>
        DateTime GetNextWeekday(DateTime input)
        {
            do
            {
                input = input.AddDays(1);
            } while (input.DayOfWeek == DayOfWeek.Saturday || input.DayOfWeek == DayOfWeek.Sunday);

            return input;
        }

        /// <summary>
        /// Get dates for every x-days starting from the start date.
        /// </summary>
        /// <returns></returns>
        RecurrenceValues GetEveryXDaysValues()
        {
            RecurrenceValues values;
            DateTime dt = base.StartDate;

            if (getNextDateValue)
            {
                do
                {
                    finalNextDateValue = dt;
                    if (finalNextDateValue > nextDateValue)
                        break;
                    dt = dt.AddDays(RegenEveryXDays);
                } while (dt <= nextDateValue.AddDays(regenEveryXDays + 3)); // Make sure it's past what may possibly be the next weekday.
                return null;
            }
            else
            {
                values = new RecurrenceValues();
                switch (base.TypeOfEndDate)
                {
                    case EndDateType.NoEndDate:
                        throw new Exception("The ability to create recurring dates with no End date is not currently available.");

                    case EndDateType.NumberOfOccurrences:

                        for (int i = 0; i < base.NumberOfOccurrences; i++)
                        {
                            values.AddDateValue(dt);
                            dt = dt.AddDays(RegenEveryXDays);
                        }
                        break;

                    case EndDateType.SpecificDate:
                        do
                        {
                            values.AddDateValue(dt);
                            dt = dt.AddDays(RegenEveryXDays);
                        } while (dt <= base.EndDate);
                        break;

                    default:
                        throw new ArgumentNullException("TypeOfEndDate", "The TypeOfEndDate property has not been set.");
                }
                return values;
            }
        }
#endregion //Private Procedures

        #region Public Fields
        /// <summary>
        /// What is the interval to generate dates. This is used to skip days in the cycle.
        /// </summary>
        public int RegenEveryXDays
        {
            get
            {
                return regenEveryXDays;
            }
        }

        /// <summary>
        /// What is the regeneration type such as Specific day of month, custom date, etc.
        /// </summary>
        public DailyRegenType RegenType
        {
            get
            {
                return regenType;
            }
        }
#endregion //Public Fields

    }




}
