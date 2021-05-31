using System.Collections.Generic;
using forest_report_api.Entities;

namespace forest_report_api.Models
{
    public class AnalyticsWithReportModel : AnalyticsModel
    {
        public Report InternalReport { get; set; }
    }

    public class AnalyticsModel
    {
        public string TitleRow { get; set; }
        public Organization Organization { get; set; }
        public List<AnalyticsItemModel> Items { get; set; }
        public Dictionary<string, decimal?> ItemsCalcValues { get; set; }
        public bool IsBold { get; set; }
    }

    public class AnalyticsItemModel
    {
        public string Index { get; set; }
        public decimal? Value { get; set; }
    }
}