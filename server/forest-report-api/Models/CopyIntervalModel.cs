using System.Collections.Generic;

namespace forest_report_api.Models
{
    public class CopyIntervalModel
    {
        public int Year { get; set; }
        public string ReportTypeId { get; set; }
        public string FromOrganizationId { get; set; }
        public IEnumerable<string> ToOrganizations { get; set; }
    }
}