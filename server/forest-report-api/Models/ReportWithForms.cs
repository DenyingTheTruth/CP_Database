using System.Collections.Generic;
using forest_report_api.Entities;

namespace forest_report_api.Models
{
    public class ReportWithTabs
    {
        public Report Report { get; set; }
        public IEnumerable<BaseFormRep> Tabs { get; set; }
    }
}