using System;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;

namespace forest_report_api.Models
{
    public class UserIntervalModel : UserCheckinInterval
    {
        public bool IsOverdue => EndDate < DateTime.Now;
        public bool CanBeCreated { get; set; }
    }

    public class FullUserIntervalModel : UserIntervalModel
    {
        public Report Report { get; set; }
        public bool IsFree => Report == null || Report.StatusReport == StatusReport.New;
    }
}