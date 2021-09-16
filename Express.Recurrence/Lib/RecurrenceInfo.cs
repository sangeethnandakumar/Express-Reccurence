using System;
using System.Collections.Generic;
using System.Text;

namespace Express.Recurrence.Lib
{
    public enum RecurrenceType { NotSet = -1, Daily = 0, Weekly, Monthly, Yearly };

    public class RecurrenceInfo
    {
        EndDateType endDateType = EndDateType.NotDefined;
        int numberOfOccurrences;
        int adjustmentValue;
        string seriesInfo;

        DateTime startDate;
        DateTime? endDate = null;
        RecurrenceType recurrenceType = RecurrenceType.NotSet;

        #region Internal Daily-Specific
        DailyRegenType dailyRegenType;
        int dailyRegenEveryXDays;

        internal void SetDailyRegenEveryXDays(int dailyRegenEveryXDays)
        {
            this.dailyRegenEveryXDays = dailyRegenEveryXDays;
        }

        internal void SetDailyRegenType(DailyRegenType dailyRegenType) 
        { 
            this.dailyRegenType = dailyRegenType; 
        }
#endregion //Internal Daily-Specific
        #region Public Daily-Specific Fields
        public int DailyRegenEveryXDays
        {
            get
            {
                return dailyRegenEveryXDays;
            }
        }

        public DailyRegenType DailyRegenType 
        { 
            get 
            { 
                return dailyRegenType; 
            } 
        }
#endregion //Public Daily-Specific Fields

        #region Internal Weekly-Specific

        SelectedDayOfWeekValues selectedDayOfWeekValues;
        WeeklyRegenType weeklyRegenType;
        int regenEveryXWeeks;

        internal void SetRegenEveryXWeeks(int regenEveryXWeeks)
        {
            this.regenEveryXWeeks = regenEveryXWeeks;
        }

        internal void SetWeeklyRegenType(WeeklyRegenType weeklyRegenType)
        {
            this.weeklyRegenType = weeklyRegenType;
        }

        internal void SetSelectedDayOfWeekValues(SelectedDayOfWeekValues selectedDayOfWeekValues)
        {
            this.selectedDayOfWeekValues = selectedDayOfWeekValues;
        }

        #endregion //Internal Weekly-Specific
        #region Public Weekly-Specific

        public int WeeklyRegenEveryXWeeks
        {
            get
            {
                return regenEveryXWeeks;
            }
        }

        public WeeklyRegenType WeeklyRegenType
        {
            get
            {
                return weeklyRegenType;
            }
        }

        public SelectedDayOfWeekValues WeeklySelectedDays
        {
            get
            {
                return selectedDayOfWeekValues;
            }
        }

        #endregion //Public Weekly-Specific

        #region Internal Monthly-Specific

        MonthlyRegenType monthlyRegenType;
        MonthlySpecificDatePartOne monthlySpecificDatePartOne;
        MonthlySpecificDatePartTwo monthlySpecificDatePartTwo;
        int monthlyRegenerateOnSpecificDateDayValue;
        int regenEveryXMonths;

        internal void SetRegenEveryXMonths(int regenEveryXMonths)
        {
            this.regenEveryXMonths = regenEveryXMonths;
        }

        internal void SetMonthlyRegenerateOnSpecificDateDayValue(int monthlyRegenerateOnSpecificDateDayValue)
        {
            this.monthlyRegenerateOnSpecificDateDayValue = monthlyRegenerateOnSpecificDateDayValue;
        }

        internal void SetMonthlySpecificDatePartTwo(MonthlySpecificDatePartTwo monthlySpecificDatePartTwo)
        {
            this.monthlySpecificDatePartTwo = monthlySpecificDatePartTwo;
        }

        internal void SetMonthlySpecificDatePartOne(MonthlySpecificDatePartOne monthlySpecificDatePartOne)
        {
            this.monthlySpecificDatePartOne = monthlySpecificDatePartOne;
        }

        internal void SetMonthlyRegenType(MonthlyRegenType monthlyRegenType)
        {
            this.monthlyRegenType = monthlyRegenType;
        }

        #endregion //Internal Monthly-Specific
        #region Public Monthly-Specific Fields

        /// <summary>
        ///     What is the interval to generate dates. This is used to skip months in the cycle.
        /// </summary>
        /// <value>
        ///     <para>
        ///         Integer of the interval value. 1 = every month, 2 = every other month, etc.
        ///     </para>
        /// </value>
        /// <remarks>
        ///     
        /// </remarks>
        public int MonthlyRegenEveryXMonths
        {
            get
            {
                return regenEveryXMonths;
            }
        }

        /// <summary>
        ///     Day of month to regenerate when RegenType = specific day of month.
        /// </summary>
        /// <value>
        ///     <para>
        ///         Integer of the day of the month.
        ///     </para>
        /// </value>
        /// <remarks>
        ///     
        /// </remarks>
        public int MonthlyRegenerateOnSpecificDateDayValue
        {
            get
            {
                return monthlyRegenerateOnSpecificDateDayValue;
            }
        }

        /// <summary>
        ///     What is the second part to the Custom date such as which weekday, weekend day, etc.
        /// </summary>
        /// <value>
        ///     <para>
        ///         
        ///     </para>
        /// </value>
        /// <remarks>
        ///     
        /// </remarks>
        public MonthlySpecificDatePartTwo MonthlySpecificDatePartTwo
        {
            get
            {
                return monthlySpecificDatePartTwo;
            }
        }

        /// <summary>
        ///     What is the first part to the Custom date such as First, Last.
        /// </summary>
        /// <value>
        ///     <para>
        ///         
        ///     </para>
        /// </value>
        /// <remarks>
        ///     
        /// </remarks>
        public MonthlySpecificDatePartOne MonthlySpecificDatePartOne
        {
            get
            {
                return monthlySpecificDatePartOne;
            }
        }

        /// <summary>
        ///     What is the regeneration type such as Specific day of month, custom date, etc.
        /// </summary>
        /// <value>
        ///     <para>
        ///         
        ///     </para>
        /// </value>
        /// <remarks>
        ///     
        /// </remarks>
        public MonthlyRegenType MonthlyRegenType
        {
            get
            {
                return monthlyRegenType;
            }
        }

        #endregion //Public Monthly-Specific Fields

        #region Public Yearly-Specific Fields

        public YearlySpecificDatePartOne YearlySpecificDatePartOne
        {
            get
            {
                return yearlySpecificDatePartOne;
            }
        }

        public YearlySpecificDatePartTwo YearlySpecificDatePartTwo
        {
            get
            {
                return yearlySpecificDatePartTwo;
            }
        }

        public YearlySpecificDatePartThree YearlySpecificDatePartThree
        {
            get
            {
                return yearlySpecificDatePartThree;
            }
        }

        public YearlyRegenType YearlyRegenType
        {
            get
            {
                return yearlyRegenType;
            }
        }

        public int SpecificDateDayValue
        {
            get
            {
                return specificDateDayValue;
            }
        }

        public int SpecificDateMonthValue
        {
            get
            {
                return specificDateMonthValue;
            }
        }

        #endregion //Yearly Public Fields
        #region Internal Yearly-Specific

        int specificDateDayValue;
        int specificDateMonthValue;
        YearlyRegenType yearlyRegenType = YearlyRegenType.NotSet;
        YearlySpecificDatePartOne yearlySpecificDatePartOne = YearlySpecificDatePartOne.NotSet;
        YearlySpecificDatePartTwo yearlySpecificDatePartTwo = YearlySpecificDatePartTwo.NotSet;
        YearlySpecificDatePartThree yearlySpecificDatePartThree = YearlySpecificDatePartThree.NotSet;

        internal void SetSpecificDateDayValue(int specificDateDayValue)
        {
            this.specificDateDayValue = specificDateDayValue;
        }

        internal void SetSpecificDateMonthValue(int specificDateMonthValue)
        {
            this.specificDateMonthValue = specificDateMonthValue;
        }

        internal void SetYearlyRegenType(YearlyRegenType yearlyRegenType)
        {
            this.yearlyRegenType = yearlyRegenType;
        }

        internal void SetYearlySpecificDatePartOne(YearlySpecificDatePartOne yearlySpecificDatePartOne)
        {
            this.yearlySpecificDatePartOne = yearlySpecificDatePartOne;
        }

        internal void SetYearlySpecificDatePartTwo(YearlySpecificDatePartTwo yearlySpecificDatePartTwo)
        {
            this.yearlySpecificDatePartTwo = yearlySpecificDatePartTwo;
        }

        internal void SetYearlySpecificDatePartThree(YearlySpecificDatePartThree yearlySpecificDatePartThree)
        {
            this.yearlySpecificDatePartThree = yearlySpecificDatePartThree;
        }

        #endregion //Internal Yearly-Specific

        #region Internal Global Setters

        internal void SetSeriesInfo(string seriesInfo)
        {
            this.seriesInfo = seriesInfo;
        }

        internal void SetAdjustmentValue(int adjustmentValue)
        {
            this.adjustmentValue = adjustmentValue;
        }

        internal void SetEndDateType(EndDateType endDateType)
        {
            this.endDateType = endDateType;
        }

        internal void SetNumberOfOccurrences(int numberOfOccurrences)
        {
            this.numberOfOccurrences = numberOfOccurrences;
        }

        internal void SetStartDate(DateTime startDate)
        {
            this.startDate = startDate;
        }

        internal void SetEndDate(DateTime? endDate)
        {
            this.endDate = endDate;
        }

        internal void SetRecurrenceType(RecurrenceType recurrenceType)
        {
            this.recurrenceType = recurrenceType;
        }

        #endregion //Internal Gloal Setters

        #region Constructors

        internal RecurrenceInfo()
        {
        }

        #endregion //Constructors

        #region Public Global Fields

        public string SeriesInfo
        {
            get
            {
                return seriesInfo;
            }
        }

        public int AdjustmentValue
        {
            get
            {
                return adjustmentValue;
            }
        }

        public RecurrenceType RecurrenceType
        {
            get
            {
                return recurrenceType;
            }
        }

        public bool HasEndDate
        {
            get
            {
                return endDate.HasValue;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                if (endDate.HasValue)
                    return endDate.Value;

                return null;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
        }

        public int NumberOfOccurrences
        {
            get
            {
                return numberOfOccurrences;
            }
        }

        public EndDateType EndDateType
        {
            get
            {
                return endDateType;
            }
        }

        #endregion //Public Global Fields


    }
}
