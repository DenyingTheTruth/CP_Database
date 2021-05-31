using System;

namespace forest_report_api.Helper
{
    public static class DateHelper
    {
        public static DateTime GetStart(string type, int year)
        {
            if (type == "quarter")
                return new DateTime(year, 1, 1);
            else
                return new DateTime(1, 1, 1);
        }

        public static DateTime GetEnd(string type, int year, int? numberQuarter = null)
        {
            if (type == "quarter" && numberQuarter.HasValue)
                    return numberQuarter switch
                    {
                        1 => new DateTime(year, 3, DateTime.DaysInMonth(year, 3)),
                        2 => new DateTime(year, 6, DateTime.DaysInMonth(year, 6)),
                        3 => new DateTime(year, 9, DateTime.DaysInMonth(year, 9)),
                        4 => new DateTime(year, 12, DateTime.DaysInMonth(year, 12)),
                        _ => throw new ArgumentOutOfRangeException(nameof(numberQuarter), numberQuarter, null)
                    };
            return DateTime.Now;
        }
    }
}