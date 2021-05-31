using System;
using System.Linq;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;

namespace forest_report_api.Extensions
{
    public static class OrganizationExtensions
    {
        public static object GetFrontObject(this Organization sender)
        {
            return sender != null ? new
            {
                Id = sender.Id,
                Name = sender.Name,
                Region = sender.Region,
                Users = sender.Users?.Select(x => new
                {
                    Id = x.Id,
                    UserName = x.UserName
                }),
                IsHolding = sender.IsHolding,
                IsState = sender.IsState,
                TypeActivityId = sender.TypeActivityId,
                TypeActivity = sender.TypeActivity
            } : null;
        }

        public static object GetGeneralInfo(this Organization sender)
        {
            return sender != null ? new
            {
                Id = sender.Id,
                Name = sender.Name,
                
                UNP = sender.UNP,
                TypeEconomicActivity = sender.TypeEconomicActivity,
                OrganizationalLegalForm = sender.OrganizationalLegalForm,
                GovermentForReport = sender.GovermentForReport,
                UnitForReport = sender.UnitForReport,
                Address = sender.Address,
                Position1 = sender.Position1,
                FullName1 = sender.FullName1,
                Position2 = sender.Position2,
                FullName2 = sender.FullName2
            } : null;
        }
        
        public static bool OrganizationByFilter(this Organization sender, ReportStatisticsFilterEnum filterEnum)
        {
            switch (filterEnum)
            {
                case ReportStatisticsFilterEnum.Full:
                case ReportStatisticsFilterEnum.PercentageOfStateOwnership:
                    return true;
                case ReportStatisticsFilterEnum.WithoutOrganizationGroup:
                    return !sender.Name.Contains("концерн",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.Industrial:
                    return sender.TypeActivity.IsIndustrial;
                case ReportStatisticsFilterEnum.Woodworking:
                    return sender.TypeActivity.Name.Contains(
                        "деревообрабатывающие предприятия", StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.PPIEnterprises:
                    return sender.TypeActivity.Name.Contains("предприятия цбп",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.Furniture:
                    return sender.TypeActivity.Name.Contains("мебельные предприятия",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.UnIndustrial:
                    return !sender.TypeActivity.IsIndustrial;
                case ReportStatisticsFilterEnum.LoggingOrganizations:
                    return sender.TypeActivity.Name.Contains("лесозаготовительные организации",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.TradeEnterprises:
                    return sender.TypeActivity.Name.Contains("торговые предприятия",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.Others:
                    return sender.TypeActivity.Name.Contains("прочие",
                        StringComparison.OrdinalIgnoreCase);
                case ReportStatisticsFilterEnum.PercentageMoreThenHalf:
                    return sender.IsState;
                case ReportStatisticsFilterEnum.PercentageLessThenHalf:
                    return !sender.IsState;
                case ReportStatisticsFilterEnum.IsDOHolding:
                    return sender.IsHolding;
                default:
                    return false;
            }
        }
    }
}