using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace forest_report_api.Entities
{
    public class Period : BaseEntity
    {
        public IEnumerable<PeriodReportType> ReportTypes { get; set; }
        public string Name { get; set; }
    }
}