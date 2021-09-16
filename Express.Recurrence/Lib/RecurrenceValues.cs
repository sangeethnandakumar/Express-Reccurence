using System;
using System.Collections.Generic;

namespace Express.Recurrence.Lib
{
    public class RecurrenceValues
    {
        List<DateTime> values = new List<DateTime>() ;
        DateTime endDate;
        DateTime startDate;
        string seriesInfo;

        public RecurrenceValues() { }

        /// <summary>
        /// Get a Generic.List<DateTime> of Recurrence values.
        /// </summary>
        public List<DateTime> Values
        {
            get
            {
                return values;
            }
        }

        public DateTime LastDate
        {
          	get
            {
                if (values.Count > 0)
                    return values[values.Count - 1];
                else
                    return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Add a date to the List of Values. 
        /// </summary>
        /// <param name="recurrenceDate"></param>
        internal void AddDateValue(DateTime recurrenceDate)
        {
            values.Add(recurrenceDate);
        }

        /// <summary>
        /// Add a date to the List of Values adjusting it with the plus/minus x-days value
        /// </summary>
        /// <param name="recurrenceDate"></param>
        /// <param name="adjustedValue"></param>
        internal void AddDateValue(DateTime recurrenceDate, int adjustedValue)
        {
            values.Add(recurrenceDate.AddDays(adjustedValue));
        }

        /// <summary>
        ///     Set the Start Date. Only accessable from this assembly 
        ///     and usually set when trying to get the next date or 
        ///     an existing set of dates.
        /// </summary>
        /// <param name="startingDate" type="System.DateTime">
        ///     <para>
        ///         
        ///     </para>
        /// </param>
        internal void SetStartDate(DateTime startingDate)
        {
            startDate = startingDate;
        }

        /// <summary>
        ///     Set the SeriesInfo. Only accessable from this assembly 
        ///     and usually set when trying to get the next date or 
        ///     an existing set of dates.
        /// </summary>
        /// <param name="seriesInfo" type="string">
        ///     <para>
        ///         
        ///     </para>
        /// </param>
        internal void SetSeriesInfo(string seriesInfo)
        {
            this.seriesInfo = seriesInfo;
        }

        /// <summary>
        ///     Set the End Date. Only accessable from this assembly 
        ///     and usually set when trying to get the next date or 
        ///     an existing set of dates.
        /// </summary>
        /// <param name="endingDate" type="System.DateTime">
        ///     <para>
        ///         
        ///     </para>
        /// </param>
        internal void SetEndDate(DateTime endingDate)
        {
            endDate = endingDate;
        }

        /// <summary>
        ///     Readonly Start Date value. This is the first date of the recurring values.
        /// </summary>
        /// <value>
        ///     <para>
        ///         
        ///     </para>
        /// </value>
        /// <remarks>
        ///     
        /// </remarks>
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
        }

        /// <summary>
        ///     Readonly End Date value. This is the last date in the recurring values.
        /// </summary>
        /// <value>
        ///     <para>
        ///         
        ///     </para>
        /// </value>
        /// <remarks>
        ///     
        /// </remarks>
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
        }

        /// <summary>
        /// Series Information contained in a coded string.
        /// </summary>
        public string GetSeriesInfo()
        {
            return seriesInfo;
        }
    }
}
