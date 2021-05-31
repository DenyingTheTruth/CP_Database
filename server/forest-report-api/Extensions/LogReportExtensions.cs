using System.Collections.Generic;
using System.Linq;
using forest_report_api.Entities;

namespace forest_report_api.Extensions
{
    public static class LogReportExtensions
    {
        public static object GetFrontObject(this List<LogReport> logReport)
        {
            return new
            {
                Logs = logReport.Select(x => new
                {
                    Id = x.Id,
                    Date = x.Date,
                    Report = x.Report.GetFrontObject(),
                    StatusReport = x.StatusReport,
                    AdminStatusReport = x.AdminStatusReport,
                    ApplicationUser = x.ApplicationUser.GetFrontObject()
                }),
                Years = logReport.Select(x => x.Report.UserCheckinInterval.Year).OrderBy(x => x).Distinct(),
                Periods = logReport
                    .Select(x => x.Report.UserCheckinInterval.Period.Name).OrderBy(x => x).Distinct()
            };
        }
    }
}