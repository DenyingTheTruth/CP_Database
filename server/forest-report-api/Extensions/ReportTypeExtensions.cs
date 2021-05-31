using System.Collections.Generic;
using System.Linq;
using forest_report_api.Entities;

namespace forest_report_api.Extensions
{
    public static class ReportTypeExtensions
    {
        public static object GetFrontObject(this ReportType sender)
        {
            return new
            {
                Id = sender.Id,
                Name = sender.Name,
                Periods = sender.Periods.Select(x => new
                {
                    Id = x.Period.Id,
                    Name = x.Period.Name
                })
            };
        }
    }
}