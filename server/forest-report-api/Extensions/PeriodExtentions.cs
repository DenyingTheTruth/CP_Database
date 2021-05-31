using System.Linq;
using forest_report_api.Entities;

namespace forest_report_api.Extensions
{
    public static class PeriodExtensions
    {
        public static object GetFrontObject(this Period sender)
        {
            return new
            {
                Id = sender.Id,
                Name = sender.Name,
                ReportTypes = sender.ReportTypes.Select(y => new
                {
                    Id = y.ReportType.Id,
                    Name = y.ReportType.Name
                })
            };
        }
    }
}