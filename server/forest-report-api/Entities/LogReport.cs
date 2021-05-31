using System;
using forest_report_api.Entities.Enums;

namespace forest_report_api.Entities
{
    public class LogReport : BaseEntity
    {
        public string ReportId { get; set; }
        public Report Report { get; set; }
        public DateTime Date { get; set; }
        public StatusReport? StatusReport { get; set; }
        public AdminStatusReport? AdminStatusReport { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}