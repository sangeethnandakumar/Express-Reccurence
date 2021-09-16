using System;

namespace Express.Recurrence.Lib
{
    /// <summary>
    ///     Provides a means to get values after the recurring dates have 
    ///     already been obtained. 
    ///     Use to get the next date in the series. Use to get items that
    ///     follow the same pattern as the series first created, but get
    ///     the dates past the last date in the series.
    /// </summary>
    /// <remarks>
    ///     
    /// </remarks>
    public class RecurrenceHelper
    {
        /// <summary>
        ///     Get the Series Info in a user-friendly object that can be used as a means to 
        ///     populate UI controls.
        /// </summary>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         String of the Series Info.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A RecurrenceGenerator.RecurrenceInfo value...
        /// </returns>
        public static RecurrenceInfo GetFriendlySeriesInfo(string seriesInfo)
        {
            RecurrenceInfo returnValue = null;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    returnValue = YearlyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);
                    break;

                case "M":   // Monthly
                    returnValue = MonthlyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);
                    break;

                case "W":   // Weekly
                    returnValue = WeeklyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);
                    break;

                case "D":   // Daily
                    returnValue = DailyRecurrenceSettings.GetFriendlyRecurrenceInfo(seriesInfo);
                    break;

            }
            return returnValue;
        }

        /// <summary>
        ///     Get the next date in the series given the current date in the series 
        ///     and the series information
        /// </summary>
        /// <param name="currentDate" type="System.DateTime">
        ///     <para>
        ///         The current date in the recurrence dates. This is the date just before 
        ///         the one you're trying to locate.
        ///     </para>
        /// </param>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         The recurrence pattern series information.
        ///     </para>
        /// </param>
        /// <returns>
        ///     The next date in the recurrence pattern as defined by the series 
        ///     information string.
        /// </returns>
        public static DateTime GetNextDate(System.DateTime currentDate, string seriesInfo)
        {
            RecurrenceSettings returnValues = null;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    returnValues = YearlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "M":   // Monthly
                    returnValues = MonthlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "W":   // Weekly
                    returnValues = WeeklyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "D":   // Daily
                    returnValues = DailyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

            }
            // Return just the next date. The function "GetNextDate" is an abstract 
            // method overriden in each of the RecurrenceSettings classes.
            return returnValues.GetNextDate(currentDate);
        }

        /// <summary>
        ///     Get the Values for a specific recurrence series by passing in the series info 
        ///     that defines the recurrence patter.
        /// </summary>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         Recurrence pattern information.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A BOCA.RecurrenceGenerator.RecurrenceValues value...
        /// </returns>
        public static RecurrenceValues GetRecurrenceValues(string seriesInfo)
        {
            RecurrenceSettings settings = null;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    settings = YearlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "M":   // Monthly
                    settings = MonthlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "W":   // Weekly
                    settings = WeeklyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "D":   // Daily
                    settings = DailyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

            }
            // Return the RecurrenceValues
            return settings.GetValues();
        }

        /// <summary>
        /// Get recurrence values using an existin Series Info value as well as a modified Start Date.
        /// </summary>
        /// <param name="modifiedStartDateValue"></param>
        /// <param name="seriesInfo"></param>
        /// <returns></returns>
        public static RecurrenceValues GetRecurrenceValues(DateTime modifiedStartDateValue, string seriesInfo)
        {
            RecurrenceSettings settings = null;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    settings = YearlyRecurrenceSettings.GetRecurrenceSettings(modifiedStartDateValue, seriesInfo);
                    break;

                case "M":   // Monthly
                    settings = MonthlyRecurrenceSettings.GetRecurrenceSettings(modifiedStartDateValue, seriesInfo);
                    break;

                case "W":   // Weekly
                    settings = WeeklyRecurrenceSettings.GetRecurrenceSettings(modifiedStartDateValue, seriesInfo);
                    break;

                case "D":   // Daily
                    settings = DailyRecurrenceSettings.GetRecurrenceSettings(modifiedStartDateValue, seriesInfo);
                    break;

            }
            // Return the RecurrenceValues
            return settings.GetValues();
        }

        /// <summary>
        ///     Get the recurrence values using an existing Series Info value and modified Occurrence Values.
        ///     This is used to get a modified set of values where you want the occurrence count to be different.
        /// </summary>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         The recurrence pattern series information.
        ///     </para>
        /// </param>
        /// <param name="modifiedOccurrencesValue" type="int">
        ///     <para>
        ///         Integer value of the modified occurrence count.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A RecurrenceGenerator.RecurrenceValues value...
        /// </returns>
        public static RecurrenceValues GetRecurrenceValues(string seriesInfo, int modifiedOccurrencesValue)
        {
            RecurrenceSettings settings = null;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    settings = YearlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue);
                    break;

                case "M":   // Monthly
                    settings = MonthlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue);
                    break;

                case "W":   // Weekly
                    settings = WeeklyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue);
                    break;

                case "D":   // Daily
                    settings = DailyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedOccurrencesValue);
                    break;

            }
            
            // Return the RecurrenceValues
            return settings.GetValues();
        }

        /// <summary>
        /// Get the reccurrence values using an existing SeriesInfo value but with adjusted number of occurrences and a modified start date.
        /// </summary>
        /// <param name="seriesInfo">Existing SeriesInfo value</param>
        /// <param name="modifiedStartDateValue">Modified Start Date</param>
        /// <param name="modifiedOccurrencesValue">Modified number of occurrences</param>
        /// <returns></returns>
        public static RecurrenceValues GetRecurrenceValues(string seriesInfo,DateTime modifiedStartDateValue, int modifiedOccurrencesValue)
        {
            RecurrenceSettings settings = null;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    settings = YearlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo,modifiedStartDateValue, modifiedOccurrencesValue);
                    break;

                case "M":   // Monthly
                    settings = MonthlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedStartDateValue, modifiedOccurrencesValue);
                    break;

                case "W":   // Weekly
                    settings = WeeklyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedStartDateValue, modifiedOccurrencesValue);
                    break;

                case "D":   // Daily
                    settings = DailyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedStartDateValue, modifiedOccurrencesValue);
                    break;

            }

            // Return the RecurrenceValues
            return settings.GetValues();
        }


        /// <summary>
        ///     Get a modified collection of recurrence values from an existing Series Info value and modified
        ///     End Date value.
        /// </summary>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         The recurrence pattern series information.
        ///     </para>
        /// </param>
        /// <param name="modifiedEndDateValue" type="System.DateTime">
        ///     <para>
        ///         DateTime of the modified end date that you want the recurrence values to continue until.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A RecurrenceGenerator.RecurrenceValues value...
        /// </returns>
        public static RecurrenceValues GetRecurrenceValues(string seriesInfo, DateTime modifiedEndDateValue)
        {
            RecurrenceSettings settings = null;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    settings = YearlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedEndDateValue);
                    break;

                case "M":   // Monthly
                    settings = MonthlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedEndDateValue);
                    break;

                case "W":   // Weekly
                    settings = WeeklyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedEndDateValue);
                    break;

                case "D":   // Daily
                    settings = DailyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedEndDateValue);
                    break;

            }

            // Return the RecurrenceValues
            return settings.GetValues();
        }

        public static RecurrenceValues GetRecurrenceValues(string seriesInfo, DateTime modifiedStartDateValue, DateTime modifiedEndDateValue)
        {
            RecurrenceSettings settings = null;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    settings = YearlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedStartDateValue, modifiedEndDateValue);
                    break;

                case "M":   // Monthly
                    settings = MonthlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedStartDateValue, modifiedEndDateValue);
                    break;

                case "W":   // Weekly
                    settings = WeeklyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedStartDateValue, modifiedEndDateValue);
                    break;

                case "D":   // Daily
                    settings = DailyRecurrenceSettings.GetRecurrenceSettings(seriesInfo, modifiedStartDateValue, modifiedEndDateValue);
                    break;

            }

            // Return the RecurrenceValues
            return settings.GetValues();
        }

        /// <summary>
        ///     Get a collection of recurrence values that are AFTER the values as defined by the 
        ///     Series Info value. The endDate param. defines how far past the existing end date
        ///     as defined by the Series Info.
        /// </summary>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         The recurrence pattern series information.
        ///     </para>
        /// </param>
        /// <param name="endDate" type="System.DateTime">
        ///     <para>
        ///         DateTime of the end date that is past the end date in the Series Info where
        ///         you want the collection of values to end.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A RecurrenceGenerator.RecurrenceValues value...
        /// </returns>
        public static RecurrenceValues GetPostRecurrenceValues(string seriesInfo, DateTime endDate)
        {
            RecurrenceSettings settings = null;
            RecurrenceValues tempValues;
            DateTime lastDate;
            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    settings = YearlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "M":   // Monthly
                    settings = MonthlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "W":   // Weekly
                    settings = WeeklyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "D":   // Daily
                    settings = DailyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

            }

            // get the RecurrenceValues
            tempValues = settings.GetValues();
            // Add one day to the last date so it now becomes the Start date past the last 
            // date in the series
            lastDate = tempValues.LastDate.AddDays(1);

            return settings.GetValues(lastDate, endDate);

        }

        public static RecurrenceValues GetPostRecurrenceValues(string seriesInfo, int modifiedOccurrencesValue)
        {
            RecurrenceSettings settings = null;
            RecurrenceValues tempValues;
            DateTime lastDate;
            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    settings = YearlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "M":   // Monthly
                    settings = MonthlyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "W":   // Weekly
                    settings = WeeklyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

                case "D":   // Daily
                    settings = DailyRecurrenceSettings.GetRecurrenceSettings(seriesInfo);
                    break;

            }
            
            // get the RecurrenceValues
            tempValues = settings.GetValues();
            // Add one day to the last date so it now becomes the Start date past the last 
            // date in the series
            lastDate = tempValues.LastDate.AddDays(1);

            return settings.GetValues(lastDate, modifiedOccurrencesValue);

        }

        /// <summary>
        ///     Get a static text definition of the Series Info
        /// </summary>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         Series Info string that was generated when a series of recurring dates were created.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A string value of the Series Info definition
        /// </returns>
        public static string GetPatternDefinitioin(string seriesInfo)
        {
            string value = string.Empty;

            switch (seriesInfo.Substring(0, 1))
            {
                case "Y":   // Yearly
                    value = YearlyRecurrenceSettings.GetPatternDefinition(seriesInfo);
                    break;

                case "M":   // Monthly
                    value = MonthlyRecurrenceSettings.GetPatternDefinition(seriesInfo);
                    break;

                case "W":   // Weekly
                    value = WeeklyRecurrenceSettings.GetPatternDefinition(seriesInfo);
                    break;

                case "D":   // Daily
                    value = DailyRecurrenceSettings.GetPatternDefinition(seriesInfo);
                    break;

            }

            return value;
        }
    }
}
