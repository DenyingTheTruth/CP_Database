using forest_report_api.Models;

namespace forest_report_api.Extensions
{
    public static class ReportModelExtensions
    {
        public static object GetFrontObject(this ReportModel sender)
        {
            return new
            {
                Report = sender.Report.GetFrontObject(), sender.TabModels
            };
        }
    }
}