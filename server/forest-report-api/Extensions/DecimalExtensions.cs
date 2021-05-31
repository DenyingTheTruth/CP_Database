using System;
using System.Globalization;

namespace forest_report_api.Extensions
{
    public static class DecimalExtensions
    {
        public static string GetForExcel(this decimal sender, NumberFormatInfo formatInfo = null)
        {
            return GetForExcel((decimal?) sender, formatInfo);
        }
        public static string GetForExcel(this decimal? sender, NumberFormatInfo formatInfo = null)
        {
            return sender == null
                ? "-"
                : sender < 0
                    ? formatInfo == null ? $"({Math.Abs(sender.Value)})" :
                    $"({Math.Abs(sender.Value).ToString(formatInfo)})"
                    : formatInfo == null
                        ? sender.ToString()
                        : sender.Value.ToString(formatInfo);
        }
    }
}