namespace forest_report_api.Entities
{
    public class PeriodReportType
    {
        public string PeriodId { get; set; }
        public Period Period { get; set; }
        public string ReportTypeId { get; set; }
        public ReportType ReportType { get; set; }
    }
}