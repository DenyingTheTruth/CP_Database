using System;
using System.Linq;
using forest_report_api.Entities;
using forest_report_api.Helper;
using forest_report_api.Models;

namespace forest_report_api.Extensions
{
    public static class UserCheckinIntervalExtensions
    {
        public static object GetFrontObject(this UserIntervalModel sender)
        {
            return new
            {
                Id = sender.Id,
                Date = sender.Date,
                Year = sender.Year,
                EndDate = sender.EndDate,
                Period = new
                {
                    Id = sender.Period.Id,
                    Name = sender.Period.Name,
                    ReportTypes = sender.Period.ReportTypes.Select(type => new
                    {
                        Id = type.ReportTypeId,
                        Name = type.ReportType.Name
                    }),
                    StartDate = DateHelper.GetStart("quarter", sender.Year),
                    EndDate = DateHelper.GetEnd("quarter", sender.Year,
                        int.Parse(sender.Period.Name[sender.Period.Name.Length - 1].ToString())),
                },
                IsOverdue = sender.IsOverdue,
                CanBeCreated = sender.CanBeCreated
            };
        }
    }
}