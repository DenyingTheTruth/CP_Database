using System.Collections.Generic;
using forest_report_api.Entities;

namespace forest_report_api.Models
{
    public class ReportModel
    {
        public string PeriodId { get; set; }
        public Report Report { get; set; }
        public List<TabModel> TabModels { get; set; }
        public List<Organization> ValidOrganization { get; set; }
        public List<Organization> InvalidOrganization { get; set; }
    }
}