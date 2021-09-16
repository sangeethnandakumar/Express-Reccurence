using System;

namespace Express.Recurrence
{
    public class DayReccurance : ReccurancePattern
    {
        public Every Every { get; set; }
        public int Next { get; set; }
    }

    public class WeeklyReccurance : ReccurancePattern
    {
        public int Next { get; set; }
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
    }

    public class MonthlyReccurance : ReccurancePattern
    {
        public int Next { get; set; }
        public int DayOfMonth { get; set; }
        public CONSTRAIN Constrain { get; set; }
        public LOGICALSTART LogicalStart { get; set; }
        public LOGICALDAY LogicalDay { get; set; }
        public int AdjustDays { get; set; }
    }

    public class YearlyReccurance : ReccurancePattern
    {
        public int DayOfMonth { get; set; }
        public MONTHS Month { get; set; }
        public CONSTRAIN Constrain { get; set; }
        public LOGICALSTART LogicalStart { get; set; }
        public LOGICALDAY LogicalDay { get; set; }
        public int AdjustDays { get; set; }
    }

    public class ReccurancePattern
    {
        public DateTime StartDate { get; set; }
        public int MaxOccurance { get; set; }
        public EndMode EndMode { get; set; }
        public DateTime EndDate { get; set; }
    }

    public enum EndMode
    {
        ByOccurance,
        ByDate
    }

    public enum CONSTRAIN
    {
        SpecificDay,
        Logical,
    }
    public enum LOGICALSTART
    {
        First,
        Second,
        Third,
        Fourth,
        Last
    }
    public enum LOGICALDAY
    {
        Day, 
        Weekday, 
        WeekendDay, 
        Sunday, 
        Monday, 
        Tuesday, 
        Wednesday, 
        Thursday, 
        Friday, 
        Saturday
    }

    public enum Every
    {
        NDay,
        WeekDay
    }

    public enum MONTHS
    {
        Jan,
        Feb,
        Mar,
        Apr,
        May,
        Jun,
        Jul,
        Aug,
        Sep,
        Oct,
        Nov,
        Dec,
    }
}
