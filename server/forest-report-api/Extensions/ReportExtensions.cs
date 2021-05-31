using System;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;

namespace forest_report_api.Extensions
{
    public static class ReportExtensions
    {
        public static object GetFrontObject(this Report sender)
        {
            return new
            {
                Id = sender.Id,
                Date = sender.Date,
                ReplyDate = sender.ReplyDate,
                StatusReport = sender.StatusReport,
                AdminStatusReport = sender.AdminStatusReport,
                UserId = sender.UserId,
                ReportType = sender.ReportType != null
                    ? new
                    {
                        Id = sender.ReportType.Id,
                        Name = sender.ReportType.Name
                    }
                    : null,
                UserCheckinInterval = sender.UserCheckinInterval != null
                    ? new
                    {
                        Id = sender.UserCheckinInterval.Id,
                        Organization = sender.UserCheckinInterval.Organization != null
                            ? new
                            {
                                Id = sender.UserCheckinInterval.Organization.Id,
                                Name = sender.UserCheckinInterval.Organization.Name,
                                IsHolding = sender.UserCheckinInterval.Organization.IsHolding,
                                IsState = sender.UserCheckinInterval.Organization.IsState,
                                TypeActivityId = sender.UserCheckinInterval.Organization.TypeActivityId,
                                Region = sender.UserCheckinInterval.Organization.Region
                            }
                            : null,
                        PeriodId = sender.UserCheckinInterval.PeriodId,
                        Period = sender.UserCheckinInterval.Period != null
                            ? new
                            {
                                Id = sender.UserCheckinInterval.Period.Id,
                                Name = sender.UserCheckinInterval.Period.Name
                            }
                            : null,
                        Year = sender.UserCheckinInterval.Year,
                        EndDate = sender.UserCheckinInterval.EndDate
                    }
                    : null,
                AttachmentFile = sender.AttachmentFile != null
                    ? new
                    {
                        Id = sender.AttachmentFile.Id,
                        Name = sender.AttachmentFile.Name,
                        Type = sender.AttachmentFile.Type,
                        Value = sender.AttachmentFile.Value
                    }
                    : null,
                Note = sender.Note,
                CreationDate = sender.CreationDate,
                IsRead = sender.IsRead,
            };
        }

        public static object GetMiniObject(this Report sender)
        {
            return new
            {
                Id = sender.Id,
                StatusReport = sender.StatusReport,
                AdminStatusReport = sender.AdminStatusReport,
                AttachmentFile = sender.AttachmentFile != null
                    ? new
                    {
                        Id = sender.AttachmentFile.Id,
                        Name = sender.AttachmentFile.Name,
                        Type = sender.AttachmentFile.Type,
                        Value = sender.AttachmentFile.Value
                    }
                    : null
            };
        }

        public static bool ShowOnStatisticsByFilter(this Report report, ReportStatisticsFilterEnum filterEnum)
        {
            switch (filterEnum)
            {
                case ReportStatisticsFilterEnum.Full:
                case ReportStatisticsFilterEnum.PercentageOfStateOwnership:
                    return true;
                case ReportStatisticsFilterEnum.WithoutOrganizationGroup:
                    return !report.UserCheckinInterval.Organization.Name.Contains("концерн",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.Industrial:
                    return report.UserCheckinInterval.Organization.TypeActivity.IsIndustrial;
                case ReportStatisticsFilterEnum.Woodworking:
                    return report.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                        "деревообрабатывающие предприятия", StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.PPIEnterprises:
                    return report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("предприятия цбп",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.Furniture:
                    return report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("мебельные предприятия",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.UnIndustrial:
                    return !report.UserCheckinInterval.Organization.TypeActivity.IsIndustrial;
                case ReportStatisticsFilterEnum.LoggingOrganizations:
                    return report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("лесозаготовительные организации",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.TradeEnterprises:
                    return report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("торговые предприятия",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.Others:
                    return report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("прочие",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.PercentageMoreThenHalf:
                    return report.UserCheckinInterval.Organization.IsState;
                case ReportStatisticsFilterEnum.PercentageLessThenHalf:
                    return !report.UserCheckinInterval.Organization.IsState;
                case ReportStatisticsFilterEnum.IsDOHolding:
                    return report.UserCheckinInterval.Organization.IsHolding;
                default:
                    return false;
            }
        }
    }
}