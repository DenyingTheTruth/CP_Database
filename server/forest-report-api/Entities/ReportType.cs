using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace forest_report_api.Entities
{
    public class ReportType : BaseEntity
    {
        public IEnumerable<PeriodReportType> Periods { get; set; }
        public string Name { get; set; }
    }
}