using System;
using System.Collections.Generic;
using forest_report_api.Entities.Enums;

namespace forest_report_api.Models
{
    public class ReportStatisticsFilterModel
    {
        public int Year { get; set; }
        public string PeriodId { get; set; }
        public string ReportTypeId { get; set; }
        public ReportStatisticsFilterEnum[] Filter { get; set; }
    }
}