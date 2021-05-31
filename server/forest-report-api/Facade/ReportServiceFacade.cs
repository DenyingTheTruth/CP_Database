using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;
using forest_report_api.Extensions;
using forest_report_api.Helper;
using forest_report_api.Models;
using forest_report_api.Repositories;
using forest_report_api.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ubiety.Dns.Core.Records.NotUsed;

namespace forest_report_api.Facade
{
    public class ReportServiceFacade
    {
        private readonly IReportTabRepository _reportTabRepository;
        private readonly IReportService _reportService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IUserIntervalService _userIntervalService;

        public ReportServiceFacade(IReportTabRepository reportTabRepository,
            IReportService reportService,
            IOrganizationRepository organizationRepository,
            IPeriodRepository periodRepository,
            IUserIntervalService userIntervalService)
        {
            _reportTabRepository = reportTabRepository;
            _reportService = reportService;
            _organizationRepository = organizationRepository;
            _periodRepository = periodRepository;
            _userIntervalService = userIntervalService;
        }

        public async Task<object> GetLogReports(string userId = null) =>
            (await _reportService.GetLogReports(userId)).GetFrontObject();

        public async Task<object> GetReports(string userId = null) =>
            (await _reportService.GetReports(userId)).Select(x => x.GetFrontObject());

        public async Task<object> GetReportsRevision(string userId = null) =>
            (await _reportService.GetReportsRevision(userId)).Select(x => x.GetFrontObject());

        public async Task<object> GetSentReports() =>
            (await _reportService.GetSentReports()).Select(x => x.GetFrontObject());

        public async Task<int> GetCountReportsRevision(string userId = null) =>
            (await _reportService.GetReportsRevision(userId)).Count;

        public async Task<int> GetCountUnreadReports() =>
            await _reportService.GetCountUnreadReports();

        public async Task<TabModel> GetTab(string organizationId, string reportTypeName, int? number = null,
            string intervalId = null)
        {
            UserCheckinInterval quarter = null;
            if (intervalId != null)
                quarter = await _userIntervalService.GetUserIntervalById(intervalId);
            var tabModel = new Tabs(int.Parse(quarter?.Period?.Name[quarter.Period.Name.Length - 1].ToString() ?? "0"),
                    quarter?.Year).Models
                .FirstOrDefault(x => x.ReportTypeName == reportTypeName &&
                                     (number != null && x.TabId == number || number == null && x.TabId == 1))
                .DeepClone();
            var organization = !string.IsNullOrEmpty(organizationId)
                            ? await _organizationRepository.GetById(organizationId)
                            : quarter?.Organization;
            if (tabModel != null && organization != null)
            {
                tabModel.Header.Organization = organization.Name;
                tabModel.Header.Number = organization.UNP;
                tabModel.Header.TypeEconomicActivity = organization.TypeEconomicActivity;
                tabModel.Header.OrganizationalLegalForm = organization.OrganizationalLegalForm;
                tabModel.Header.Government = organization.GovermentForReport;
                tabModel.Header.Unit = organization.UnitForReport;
                tabModel.Header.Address = organization.Address;

                tabModel.Footer.LeaderName = organization.Position1;
                tabModel.Footer.AccountantGeneral = organization.Position2;
                tabModel.Footer.Leader = organization.FullName1;
                tabModel.Footer.ChiefAccountant = organization.FullName2;
            }

            return tabModel;
        }

        public object GetTabs(string reportTypeName)
        {
            var tabModels = new Tabs().Models.Where(x =>
                x.ReportTypeName == reportTypeName).ToList();
            return new
            {
                Count = tabModels.Count(),
                Names = tabModels.Select(x => x.Title)
            };
        }

        public async Task<object> GetReport(string id)
        {
            var report = await _reportService.GetFullReport(id);
            return report != null ? report.GetFrontObject() : new ResponseResult(false, "Not found");
        }

        public async Task<object> GetReportStatisticsByFilter(ReportStatisticsFilterModel filter)
        {
            try
            {
                return await _reportService.GetReportStatisticsByFilter(filter);
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> GetReportStatisticsByMultiFilter(ReportStatisticsFilterModel filter)
        {
            try
            {
                return await _reportService.GetReportStatisticsByMultiFilter(filter);
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> GetFile(string reportId)
        {
            try
            {
                var file = (await _reportService.GetReport(reportId)).AttachmentFile;
                return new FileContentResult(file.Value, file.Type)
                {
                    FileDownloadName = $"{file.Name}"
                };
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<ResponseResult> SaveReport(ReportModel model, string userId)
        {
            try
            {
                var org = await _organizationRepository.GetByUserId(userId);
                if (org != null)
                {
                    model.TabModels.ForEach(x =>
                    {
                        x.Header.Organization = org.Name;
                        x.Header.Number = org.UNP;
                        x.Header.TypeEconomicActivity = org.TypeEconomicActivity;
                        x.Header.OrganizationalLegalForm = org.OrganizationalLegalForm;
                        x.Header.Government = org.GovermentForReport;
                        x.Header.Unit = org.UnitForReport;
                        x.Header.Address = org.Address;

                        x.Footer.LeaderName = org.Position1;
                        x.Footer.AccountantGeneral = org.Position2;
                        x.Footer.Leader = org.FullName1;
                        x.Footer.ChiefAccountant = org.FullName2;
                    });
                }

                var attachmentFile = model.Report.AttachmentFile == null
                    ? null
                    : new AttachmentFile()
                    {
                        Id = model.Report.AttachmentFile.IsNew
                            ? Guid.NewGuid().ToString()
                            : model.Report.AttachmentFile.Id,
                        Name = model.Report.AttachmentFile.Name,
                        Type = model.Report.AttachmentFile.Type,
                        Value = model.Report.AttachmentFile.Value
                    };
                var reportsByInterval =
                    await _reportService.GetReportsByInterval(model.Report.UserCheckinIntervalId ??
                                                              model.Report.UserCheckinInterval.Id) ??
                    new List<Report>();
                if (model.Report.IsNew)
                {
                    var collectionId = Guid.NewGuid().ToString();
                    var report = new Report
                    {
                        FormCollectionId = collectionId,
                        ReportTypeId = model.Report.ReportTypeId,
                        StatusReport = StatusReport.New,
                        UserId = userId,
                        UserCheckinIntervalId = model.Report.UserCheckinIntervalId,
                        AttachmentFile = attachmentFile
                    };

                    if (reportsByInterval.Any())
                        return new ResponseResult(false, "1488",
                            reportsByInterval.FirstOrDefault(x => x.StatusReport == StatusReport.New)?.Id);

                    // save by reportType and validate
                    await SaveTabs(model.TabModels, collectionId);
                    await _reportService.SaveReport(report, userId);
                    return new ResponseResult(true, "Report saved", report.Id);
                }
                else
                {
                    var existReport = await _reportService.GetReport(model.Report.Id);
                    existReport.AttachmentFile =
                        attachmentFile == null || attachmentFile.Id != existReport.AttachmentFile?.Id
                            ? attachmentFile
                            : existReport.AttachmentFile;
                    existReport.Note = model.Report.Note;
                    // save by reportType and validate
                    await SaveTabs(model.TabModels, existReport.FormCollectionId);
                    await _reportService.SaveReport(existReport, userId);
                    return new ResponseResult(true, "Report saved", existReport.Id);
                }
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        private async Task SaveTabs(IEnumerable<TabModel> tabModels, string collectionId)
        {
            foreach (var tabModel in tabModels)
            {
                IEnumerable<object> saveRows;
                //CalculationTab(tabModel, out saveRows);

                //tabModel.Table.Rows = saveRows;
                var newId = tabModel.Id != null ? tabModel.Id : Guid.NewGuid().ToString();
                tabModel.Id = newId;
                var existModels = await _reportTabRepository.GetByCollectionId(collectionId);
                var saveModel = new BaseFormRep()
                {
                    Id = existModels.FirstOrDefault(x => x.TabId == tabModel.TabId)?.Id,
                    CollectionId = collectionId,
                    Json = JsonConvert.SerializeObject(tabModel),
                    TabId = tabModel.TabId
                };
                await _reportTabRepository.Save(saveModel);
            }
        }

        private void CalculationTab(TabModel tabModel, out IEnumerable<object> saveRows)
        {
            switch (tabModel.TabName)
            {
                case TabName.BalanceTab:
                    var rowsBalanceSheetItems =
                        new List<BalanceSheetItem>(MyConverter.ConvertToClass<BalanceSheetItem>(tabModel.Table.Rows));
                    rowsBalanceSheetItems.AutoCalculate("110", "-", "111", "112");
                    rowsBalanceSheetItems.AutoCalculate("130", "+,-", "131", "132", "133");
                    rowsBalanceSheetItems.AutoCalculate("190", "+,+,+,+,+,+,+", "110", "120", "130", "140", "150",
                        "160", "170", "180");
                    rowsBalanceSheetItems.AutoCalculate("210", "+,+,+,+,+", "211", "212", "213", "214", "215",
                        "216");
                    rowsBalanceSheetItems.AutoCalculate("290", "+,+,+,+,+,+,+", "210", "220", "230", "240", "250",
                        "260", "270", "280");
                    rowsBalanceSheetItems.AutoCalculate("300", "+", "190", "290");
                    rowsBalanceSheetItems.AutoCalculate("490", "-,-,+,+,+,+,+,+", "410", "420", "430", "440", "450",
                        "460", "470", "480");
                    rowsBalanceSheetItems.AutoCalculate("590", "+,+,+,+,+", "510", "520", "530", "540", "550", "560");
                    rowsBalanceSheetItems.AutoCalculate("630", "+,+,+,+,+,+,+", "631", "632", "633", "634", "635",
                        "636", "637", "638");
                    rowsBalanceSheetItems.AutoCalculate("690", "+,+,+,+,+,+", "610", "620", "630", "640", "650", "650",
                        "660", "670");
                    rowsBalanceSheetItems.AutoCalculate("700", "+,+", "490", "590", "690");
                    saveRows = rowsBalanceSheetItems;
                    break;
                case TabName.ProfitLossTab:
                    var rowsProfitLossItems =
                        new List<ProfitLossItem>(MyConverter.ConvertToClass<ProfitLossItem>(tabModel.Table.Rows));
                    rowsProfitLossItems.AutoCalculate("030", "-", "010", "020");
                    rowsProfitLossItems.AutoCalculate("060", "-,-", "030", "040", "050");
                    rowsProfitLossItems.AutoCalculate("090", "+,-", "060", "070", "080");
                    rowsProfitLossItems.AutoCalculate("100", "+,+,+", "101", "102", "103", "104");
                    rowsProfitLossItems.AutoCalculate("110", "+", "111", "112");
                    rowsProfitLossItems.AutoCalculate("120", "+", "121", "122");
                    rowsProfitLossItems.AutoCalculate("130", "+,+", "131", "132", "133");
                    rowsProfitLossItems.AutoCalculate("150", "+", "090", "140");
                    rowsProfitLossItems.AutoCalculate("210", "-,+,+,-,-", "150", "160", "170", "180", "190", "200");
                    rowsProfitLossItems.AutoCalculate("240", "+,+", "210", "220", "230");
                    saveRows = rowsProfitLossItems;
                    break;
                case TabName.ChangeEquityTab:
                    var changeEquityItems = new List<ChangeEquityItem>(
                        MyConverter.ConvertToClass<ChangeEquityItem>(tabModel.Table.Rows));
                    changeEquityItems.AutoCalculateColumn("Value8", "+,-,+,+,+,+", null, "Value1", "Value2", "Value3",
                        "Value4", "Value5", "Value6", "Value7");

                    changeEquityItems.AutoCalculate("040", "+,+", "010", "020", "030");
                    changeEquityItems.AutoCalculate("050", "+,+,+,+,+,+,+,+", "051", "052", "053", "054", "055", "056",
                        "057", "058", "059");
                    changeEquityItems.AutoCalculate("060", "+,+,+,+,+,+,+,+", "061", "062", "063", "064", "065", "066",
                        "067", "068", "069");
                    changeEquityItems.AutoCalculate("100", "+,+,+,+,+", "040", "050", "060", "070", "080", "090");
                    changeEquityItems.AutoCalculate("140", "+,+,+", "110", "120", "130", "131");
                    changeEquityItems.AutoCalculate("150", "+,+,+,+,+,+,+,+", "151", "152", "153", "154", "155", "156",
                        "157", "158", "159");
                    changeEquityItems.AutoCalculate("160", "+,+,+,+,+,+,+,+", "161", "162", "163", "164", "165", "166",
                        "167", "168", "169");
                    changeEquityItems.AutoCalculate("200", "+,-,+,+,+", "140", "150", "160", "170", "180", "190",
                        "167", "168", "169");
                    saveRows = changeEquityItems;
                    break;
                case TabName.MoveMoneyTab:
                    var moveMoneyItems =
                        new List<MoveMoneyItem>(MyConverter.ConvertToClass<MoveMoneyItem>(tabModel.Table.Rows));
                    moveMoneyItems.AutoCalculate("020", "+,+,+", "021", "022", "023", "024");
                    moveMoneyItems.AutoCalculate("030", "+,+,+", "031", "032", "033", "034");
                    moveMoneyItems.AutoCalculate("040", "-", "020", "030");
                    moveMoneyItems.AutoCalculate("050", "+,+,+,+", "051", "052", "053", "054", "055");
                    moveMoneyItems.AutoCalculate("060", "+,+,+", "061", "062", "063", "064");
                    moveMoneyItems.AutoCalculate("070", "-", "050", "060");
                    moveMoneyItems.AutoCalculate("080", "+,+,+", "081", "082", "083", "084");
                    moveMoneyItems.AutoCalculate("090", "+,+,+,+", "091", "092", "093", "094", "095");
                    moveMoneyItems.AutoCalculate("100", "-", "080", "090");
                    moveMoneyItems.AutoCalculate("110", "+,+", "040", "070", "100");
                    moveMoneyItems.AutoCalculate("130", "+", "110", "120");
                    saveRows = moveMoneyItems;
                    break;
                case TabName.DecodingAccruedTaxes:
                    var decodingAccruedTaxesItems = new List<DecodingAccruedTaxesItem>(
                        MyConverter.ConvertToClass<DecodingAccruedTaxesItem>(tabModel.Table.Rows));
                    saveRows = decodingAccruedTaxesItems;
                    break;
                case TabName.DecodingFixedAssets:
                    var decodingFixedAssetsItems = new List<DecodingFixedAssetsItem>(
                        MyConverter.ConvertToClass<DecodingFixedAssetsItem>(tabModel.Table.Rows));
                    decodingFixedAssetsItems.AutoCalculateColumn("Value4", "+,-", new[] {"020", "021", "100"},
                        "Value1", "Value2", "Value3");

                    decodingFixedAssetsItems.AutoCalculate("010", "+,+,+,+,+,+,+,+", "011", "012", "013", "014", "015",
                        "016", "017", "018", "019");
                    saveRows = decodingFixedAssetsItems;
                    break;
                default:
                    saveRows = null;
                    break;
            }
        }

        public async Task<ResponseResult> ValidateReport(ReportModel reportModel)
        {
            try
            {
                var intervalId = reportModel.Report.UserCheckinIntervalId ??
                                 reportModel.Report.UserCheckinInterval.Id;

                var interval = await _userIntervalService.GetUserIntervalById(intervalId);
                var validations = ValidationTab(reportModel.TabModels,
                    int.Parse(interval.Period.Name[^1].ToString()));

                return validations.Any()
                    ? new ResponseResult(false, "linkage", validations)
                    : new ResponseResult(true, "Submitted");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<ResponseResult> SendReport(ReportModel reportModel, string userId)
        {
            try
            {
                var intervalId = reportModel.Report.UserCheckinIntervalId ??
                                 reportModel.Report.UserCheckinInterval.Id;
                var reportsByInterval =
                    await _reportService.GetReportsByInterval(intervalId) ??
                    new List<Report>();
                if (reportsByInterval.Any(x => x.StatusReport != StatusReport.New &&
                                               x.StatusReport != StatusReport.ForRevision))
                    return new ResponseResult(false, "1488", reportModel.Report.Id);

                var interval = await _userIntervalService.GetUserIntervalById(intervalId);
                var validations = ValidationTab(reportModel.TabModels,
                    int.Parse(interval.Period.Name[interval.Period.Name.Length - 1].ToString()));

                if (validations.Any())
                    return new ResponseResult(false, "linkage", validations);

                var reportResult = await SaveReport(reportModel, userId);
                await _reportService.ChangeReportStatus(reportResult.Value.ToString(), StatusReport.Submitted, userId);
                return new ResponseResult(true, "Submitted");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<ResponseResult> AcceptReport(ReportModel reportModel, string userId)
        {
            try
            {
                var reportResult = await SaveReport(reportModel, userId);
                await _reportService.ChangeReportStatus(reportResult.Value.ToString(), StatusReport.Accepted, userId);
                return new ResponseResult(true, "Accepted");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<ResponseResult> SendForCorrection(ReportModel reportModel, string userId)
        {
            try
            {
                var reportResult = await SaveReport(reportModel, userId);
                await _reportService.ChangeReportStatus(reportResult.Value.ToString(), StatusReport.ForRevision,
                    userId);
                return new ResponseResult(true, "Send to correction");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<ResponseResult> SendForCorrection(string[] reportIds, string userId)
        {
            try
            {
                foreach (var reportId in reportIds)
                {
                    await _reportService.ChangeReportStatus(reportId, StatusReport.ForRevision,
                        userId);
                }

                return new ResponseResult(true, "Send to correction");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<ResponseResult> ReturnReportToCorrection(string[] reportIds, string userId)
        {
            try
            {
                foreach (var reportId in reportIds)
                {
                    await _reportService.ChangeReportStatus(reportId, StatusReport.Return, AdminStatusReport.Return,
                        userId, null, DateTime.Now);
                }

                return new ResponseResult(true, "Return to correction");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> ExportFile(ReportModel reportModel)
        {
            try
            {
                return new FileContentResult(await _reportService.Export(reportModel), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{reportModel.TabModels[0].ReportTypeName}.xlsx"
                };
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> ExportFile(string reportId)
        {
            try
            {
                var reportModel = await _reportService.GetFullReport(reportId);
                return new FileContentResult(await _reportService.Export(reportModel), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{reportModel.TabModels[0].ReportTypeName}.xlsx"
                };
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public object ImportFile(byte[] file, string organization)
        {
            try
            {
                return _reportService.ImportFile(file, organization);
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public object ImportFileByPeriod(byte[] file, string organization, int? quarter, int? year)
        {
            try
            {
                return _reportService.ImportFileByPeriod(file, organization, quarter, year);
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> ExportFileAnalytics(ReportStatisticsFilterModel filterModel,
            AnalyticsTypeEnum analyticsTypeEnum)
        {
            try
            {
                var analytics = analyticsTypeEnum switch
                {
                    AnalyticsTypeEnum.BalanceAsset => await GetBalanceAsset(filterModel,true),
                    AnalyticsTypeEnum.StructureObligations => await GetStructureObligations(filterModel,true),
                    AnalyticsTypeEnum.BalanceLiabilities => await GetBalanceSheetLiabilitiesStructure(filterModel,true),
                    AnalyticsTypeEnum.FinancialIndicators => await GetFinancialIndicators(filterModel,true),
                    AnalyticsTypeEnum.SolvencyRatios => await GetSolvencyRatios(filterModel,true),
                    AnalyticsTypeEnum.WorkingCapital => await GetStatusOfOwnWorkingCapital(filterModel,true),
                    _ => new ResponseResult(false, "Analytics type incorrect")
                };
                if (analytics is ResponseResult)
                    return analytics;

                var quarterModel = await _periodRepository.GetById(filterModel.PeriodId);
                return new FileContentResult(
                    _reportService.ExportAnalitycs((AnalyticModel) analytics, analyticsTypeEnum,
                        int.Parse(quarterModel?.Name[^1].ToString() ?? "0"), filterModel.Year),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{GetNameByAnalyticsType(analyticsTypeEnum)}.xlsx"
                };
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        private string GetNameByAnalyticsType(AnalyticsTypeEnum analyticsTypeEnum)
        {
            return analyticsTypeEnum switch
            {
                AnalyticsTypeEnum.BalanceAsset => "Анализ актива",
                AnalyticsTypeEnum.StructureObligations => "Анализ структуры обязательств",
                AnalyticsTypeEnum.BalanceLiabilities => "Анализ структуры пассива баланса",
                AnalyticsTypeEnum.FinancialIndicators => "Анализ финансовых показателей",
                AnalyticsTypeEnum.SolvencyRatios => "Коэффициенты платежеспособности",
                AnalyticsTypeEnum.WorkingCapital => "Состояние собственных оборотных средств",
                _ => string.Empty
            };
        }

        public async Task<object> GetReportByInterval(string userCheckinIntervalId)
        {
            var report = await _reportService.GetFullReportByInterval(userCheckinIntervalId);
            return report != null ? report.GetFrontObject() : new ResponseResult(false, "Not found");
        }

        public List<string> ValidationTab(IEnumerable<TabModel> models, int quarter)
        {
            var linkages = new List<string>();
            var tabModels = models.ToList();
            if (tabModels.Any() && tabModels.All(x => x.ReportTypeName == "Бухгалтерский баланс и приложения к нему"))
            {
                #region light

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("270", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.MoveMoneyTab)
                        ?.GetRow<decimal?>("130", "Value1").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 270 графы 1 с данными в отчете №4 строки 130 графы 1");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("270", "Value2") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.MoveMoneyTab)
                        ?.GetRow<decimal?>("120", "Value1").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 270 графы 2 с данными в отчете №4 строки 120 графы 1");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("470", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("210", "Value1").GetValueOrDefault() ?? 0) &&
                    quarter != 4)
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 470 графы 1 с данными в отчете №2 строки 210 графы 1");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("470", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("200", "Value7").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 470 графы 1 с данными в отчете №3 строки 200 графы 7");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("410", "Value2") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("140", "Value1").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 410 графы 2 с данными в отчете №3 строки 140 графы 1");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("410", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("200", "Value1").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 410 графы 1 с данными в отчете №3 строки 200 графы 1");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("420", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("200", "Value2").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 420 графы 1 с данными в отчете №3 строки 200 графы 2");

                // if (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("420", "Value1") != null
                //     && tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("420", "Value1")
                //     != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                //         ?.GetRow<decimal?>("200", "Value2").GetValueOrDefault() ?? 0))
                //     linkages.Add(
                //         "Произошло несовпадение данных в отчете №1 строки 420 графы 1 с данными в отчете №3 строки 200 графы 2");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("420", "Value2") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("140", "Value2").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 420 графы 2 с данными в отчете №3 строки 140 графы 2");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("430", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("200", "Value3").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 430 графы 1 с данными в отчете №3 строки 200 графы 3");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("440", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("200", "Value4").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 440 графы 1 с данными в отчете №3 строки 200 графы 4");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("440", "Value2") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("140", "Value4").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 440 графы 2 с данными в отчете №3 строки 140 графы 4");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("450", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("200", "Value5").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 450 графы 1 с данными в отчете №3 строки 200 графы 5");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("450", "Value2") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("140", "Value5").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 450 графы 2 с данными в отчете №3 строки 140 графы 5");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("460", "Value1") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("200", "Value6").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 460 графы 1 с данными в отчете №3 строки 200 графы 6");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("460", "Value2") ?? 0)
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                        ?.GetRow<decimal?>("140", "Value6").GetValueOrDefault() ?? 0))
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 460 графы 2 с данными в отчете №3 строки 140 графы 6");

                #endregion

                #region calc

                decimal? value1 = 0;
                decimal? value2 = 0;

                value1 = tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                    ?.GetRow<decimal?>("152", "Value5").GetValueOrDefault() ?? 0;
                value2 = tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                    ?.GetRow<decimal?>("162", "Value5").GetValueOrDefault() ?? 0;
                if (tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("220", "Value1") != null
                    && tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("220", "Value1")
                    != value1 - value2)
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №2 строки 220 графы 1 с данными в отчете №3 строки 152 графы 5, строки 162 графы 5");

                value1 = tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                    ?.GetRow<decimal?>("052", "Value5").GetValueOrDefault() ?? 0;
                value2 = tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                    ?.GetRow<decimal?>("062", "Value5").GetValueOrDefault() ?? 0;
                if (tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("220", "Value2") != null
                    && tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("220", "Value2")
                    != value1 - value2)
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №2 строки 220 графы 2 с данными в отчете №3 строки 52 графы 5, строки 62 графы 5");

                value1 = tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                    ?.GetRow<decimal?>("153", "Value8").GetValueOrDefault() ?? 0;
                value2 = tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                    ?.GetRow<decimal?>("163", "Value8").GetValueOrDefault() ?? 0;
                if (tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("230", "Value1") != null
                    && tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("230", "Value1")
                    != value1 - value2)
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №2 строки 230 графы 1 с данными в отчете №3 строки 153 графы 8, строки 163 графы 8");

                value1 = tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                    ?.GetRow<decimal?>("053", "Value8").GetValueOrDefault() ?? 0;
                value2 = tabModels.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab)
                    ?.GetRow<decimal?>("063", "Value8").GetValueOrDefault() ?? 0;
                if (tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("230", "Value2") != null
                    && tabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab)
                        ?.GetRow<decimal?>("230", "Value2")
                    != value1 - value2)
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №2 строки 230 графы 2 с данными в отчете №3 строки 53 графы 8, строки 63 графы 8");

                value1 = tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingFixedAssets)
                    ?.GetRow<decimal?>("010", "Value1").GetValueOrDefault() ?? 0;
                value2 = tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingFixedAssets)
                    ?.GetRow<decimal?>("020", "Value1").GetValueOrDefault() ?? 0;
                if (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("110", "Value2") !=
                    null
                    && tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("110", "Value2")
                    != value1 - value2)
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 110 графы 2 с данными в отчете №7 строки 010 графы 1, строки 020 графы 1");

                value1 = tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingFixedAssets)
                    ?.GetRow<decimal?>("010", "Value4").GetValueOrDefault() ?? 0;
                value2 = tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingFixedAssets)
                    ?.GetRow<decimal?>("020", "Value4").GetValueOrDefault() ?? 0;
                if (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("110", "Value1") !=
                    null
                    && tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("110", "Value1")
                    != value1 - value2)
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 110 графы 1 с данными в отчете №7 строки 010 графы 4, строки 020 графы 4");


                if (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("633", "Value1") !=
                    null
                    && tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("633", "Value1")
                    != (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("251", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("251", "Value2").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("252", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("252", "Value2").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("633", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("634", "Value1").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)
                        ?.GetRow<decimal?>("634", "Value2").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("100", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("100", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("100", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("110", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("110", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("110", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("120", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("120", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("120", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("150", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("150", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("150", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("160", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("160", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("160", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("190", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("190", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("190", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("200", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("200", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("200", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("220", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("220", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("220", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("230", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("230", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("230", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("240", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("240", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("240", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("250", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("250", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("250", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("260", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("260", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("260", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("270", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("270", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("270", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("271", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("271", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("271", "Value3").GetValueOrDefault() ?? 0)
                    + (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("280", "Value1").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("280", "Value2").GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingAccruedTaxes)
                        ?.GetRow<decimal?>("280", "Value3").GetValueOrDefault() ?? 0)
                )
                    linkages.Add(
                        "Произошло несовпадение данных в отчете №1 строки 633 графы 1 с данными в отчетах №1, №6");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("300", "Value1")
                        .GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("590", "Value1")
                        .GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("690", "Value1")
                        .GetValueOrDefault() ?? 0)
                    != tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingFixedAssets)
                        ?.GetRow<decimal?>("100", "Value4") &&
                    tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingFixedAssets)
                        ?.GetRow<decimal?>("100", "Value4") != null)
                    linkages.Add("Произошло несовпадение данных в отчете №7 строки 100 графы 4 с " +
                                 "данными в отчете №1 строки 300 графы 1, строки 590 графы 1, строки 690 графы 1");

                if ((tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("300", "Value2")
                        .GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("590", "Value2")
                        .GetValueOrDefault() ?? 0)
                    - (tabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab)?.GetRow<decimal?>("690", "Value2")
                        .GetValueOrDefault() ?? 0)
                    != tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingFixedAssets)
                        ?.GetRow<decimal?>("100", "Value1") &&
                    tabModels.FirstOrDefault(x => x.TabName == TabName.DecodingFixedAssets)
                        ?.GetRow<decimal?>("100", "Value1") != null)
                    linkages.Add("Произошло несовпадение данных в отчете №7 строки 100 графы 1 с " +
                                 "данными в отчете №1 строки 300 графы 2, строки 590 графы 2, строки 690 графы 2");

                #endregion
            }

            return linkages;
        }

        public async Task<object> GetBalanceAsset(ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var result = await _reportService.GetBalanceAsset(filterModel, isExport);
            return new AnalyticModel()
            {
                Tab1 = result[0],
                Tab2 = result[1]
            };
        }

        public async Task<object> GetStructureObligations(ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var result = await _reportService.GetStructureObligations(filterModel, isExport);
            return new AnalyticModel()
            {
                Tab1 = result[0]
            };
        }

        public async Task<object> GetBalanceSheetLiabilitiesStructure(ReportStatisticsFilterModel filterModel,bool isExport = false)
        {
            var result = await _reportService.GetBalanceSheetLiabilitiesStructure(filterModel,isExport);
            return new AnalyticModel()
            {
                Tab1 = result[0]
            };
        }

        public async Task<object> GetFinancialIndicators(ReportStatisticsFilterModel filterModel,bool isExport = false)
        {
            var result = await _reportService.GetFinancialIndicators(filterModel,isExport);
            return new AnalyticModel()
            {
                Tab1 = result[0]
            };
        }

        public async Task<object> GetSolvencyRatios(ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var result = await _reportService.GetSolvencyRatios(filterModel,isExport);
            return new AnalyticModel()
            {
                Tab1 = result[0]
            };
        }

        public async Task<object> GetStatusOfOwnWorkingCapital(ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var result = await _reportService.GetStatusOfOwnWorkingCapital(filterModel,isExport);
            return new AnalyticModel
            {
                Tab1 = result[0]
            };
        }

        public async Task<object> ReadReport(string id)
        {
            try
            {
                await _reportService.SetReportRead(id, read: true);
                return new ResponseResult(true, "Report read");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> UnreadReport(string id)
        {
            try
            {
                await _reportService.SetReportRead(id, read: false);
                return new ResponseResult(true, "Report unread");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> GetReportByPeriod(int year, string periodId, string reportTypeId, string organizationId)
        {
            try
            {
                var report = await _reportService.GetReportByPeriod(year, periodId, reportTypeId, organizationId);

                return report != null 
                    ? new ResponseResult(true, "Report found", report.GetFrontObject()) 
                    : new ResponseResult(false, "Report not found");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }
    }

    public class AnalyticModel
    {
        public List<Dictionary<string, string>> Tab1 { get; set; }
        public List<Dictionary<string, string>> Tab2 { get; set; }
    }
}