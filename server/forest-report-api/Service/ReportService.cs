using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using forest_report_api.Attribute;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;
using forest_report_api.Extensions;
using forest_report_api.Facade;
using forest_report_api.Helper;
using forest_report_api.Models;
using forest_report_api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace forest_report_api.Service
{
    public class ReportService : IReportService
    {
        private readonly ILogReportRepository _logReportRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IReportTabRepository _reportTabRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPeriodRepository _periodRepository;
        private readonly IUserIntervalService _userIntervalService;

        public ReportService(ILogReportRepository logReportRepository,
            IReportRepository reportRepository, IReportTabRepository reportTabRepository,
            IWebHostEnvironment webHostEnvironment,
            IPeriodRepository periodRepository,
            IUserIntervalService userIntervalService)
        {
            _logReportRepository = logReportRepository;
            _reportRepository = reportRepository;
            _reportTabRepository = reportTabRepository;
            _webHostEnvironment = webHostEnvironment;
            _periodRepository = periodRepository;
            _userIntervalService = userIntervalService;
        }

        public async Task<List<LogReport>> GetLogReports(string userId = null) =>
            await _logReportRepository.GetAll(userId);

        public async Task<LogReport> GetLogReport(string id) =>
            await _logReportRepository.GetById(id);

        public async Task<List<Report>> GetReports(string userId) =>
            await _reportRepository.GetAll(userId);

        public async Task<List<Report>> GetSentReports() =>
            await _reportRepository.GetSentReports();

        public async Task<List<Report>> GetReportsRevision(string userId = null) =>
            await _reportRepository.GetAllByStatus(StatusReport.ForRevision, userId);

        public async Task<int> GetCountUnreadReports()
        {
            return await _reportRepository.GetReportCount(x =>
                (x.AdminStatusReport == AdminStatusReport.AfterCorrection || 
                x.AdminStatusReport == AdminStatusReport.New) &&
                !x.IsRead);
        }

        public async Task SetReportRead(string id, bool read) =>
            await _reportRepository.SetReportRead(id, read);

        public async Task<List<List<Dictionary<string, string>>>> GetBalanceAsset(
            ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var analyticsList = new List<List<AnalyticsWithReportModel>>();
            var analyticsTable1 = new List<AnalyticsWithReportModel>();
            var analyticsTable2 = new List<AnalyticsWithReportModel>();
            var reportObjets = (dynamic) await _reportRepository.GetReportStatistics(filterModel);
            var reports = (Report[]) reportObjets.validReports;
            var validReportsIds = await _reportRepository.GetReportIdByMultiFilter(filterModel);

            foreach (var report in reports)
            {
                var model = new AnalyticsWithReportModel
                {
                    Organization = report.UserCheckinInterval.Organization,
                    Items = new List<AnalyticsItemModel>(),
                    TitleRow = report.UserCheckinInterval.Organization.Name,
                    InternalReport = report
                };
                var model2 = new AnalyticsWithReportModel
                {
                    Organization = report.UserCheckinInterval.Organization,
                    Items = new List<AnalyticsItemModel>(),
                    TitleRow = report.UserCheckinInterval.Organization.Name,
                    InternalReport = report
                };
                var fullReport = await GetFullReport(report.Id);
                var tabModel = fullReport.TabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab);
                if (tabModel != null)
                {
                    var rows = MyConverter.ConvertToClass<BalanceSheetItem>(tabModel.Table.Rows).ToList();

                    #region Fill table 1

                    model.ItemsCalcValues = new Dictionary<string, decimal?>
                    {
                        {"110V1", rows.FirstOrDefault(x => x.CodeItem == "110")?.Value1},
                        {"110V2", rows.FirstOrDefault(x => x.CodeItem == "110")?.Value2},
                        {"150V1", rows.FirstOrDefault(x => x.CodeItem == "150")?.Value1},
                        {"150V2", rows.FirstOrDefault(x => x.CodeItem == "150")?.Value2},
                        {"170V1", rows.FirstOrDefault(x => x.CodeItem == "170")?.Value1},
                        {"170V2", rows.FirstOrDefault(x => x.CodeItem == "170")?.Value2},
                        {"190V1", rows.FirstOrDefault(x => x.CodeItem == "190")?.Value1},
                        {"190V2", rows.FirstOrDefault(x => x.CodeItem == "190")?.Value2},
                        {"300V1", rows.FirstOrDefault(x => x.CodeItem == "300")?.Value1},
                        {"300V2", rows.FirstOrDefault(x => x.CodeItem == "300")?.Value2},
                    };

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "2",
                        Value = model.ItemsCalcValues["190V2"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "3",
                        Value = model.ItemsCalcValues["190V2"] / model.ItemsCalcValues["300V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "4",
                        Value = model.ItemsCalcValues["190V1"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "5",
                        Value = model.ItemsCalcValues["190V1"] / model.ItemsCalcValues["300V1"] * 100
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "6",
                        Value = model.Items[2].Value - model.Items[0].Value
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "7",
                        Value = model.Items[3].Value - model.Items[1].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "8",
                        Value = model.ItemsCalcValues["110V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "9",
                        Value = model.ItemsCalcValues["110V2"] / model.ItemsCalcValues["300V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "10",
                        Value = model.ItemsCalcValues["110V1"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "11",
                        Value = model.ItemsCalcValues["110V1"] / model.ItemsCalcValues["300V1"] * 100
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "12",
                        Value = model.Items[8].Value - model.Items[6].Value
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "13",
                        Value = model.Items[9].Value - model.Items[7].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "14",
                        Value = model.ItemsCalcValues["150V2"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "15",
                        Value = model.ItemsCalcValues["150V2"] / model.ItemsCalcValues["300V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "16",
                        Value = model.ItemsCalcValues["150V1"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "17",
                        Value = model.ItemsCalcValues["150V1"] / model.ItemsCalcValues["300V1"] * 100
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "18",
                        Value = model.Items[14].Value - model.Items[12].Value
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "19",
                        Value = model.Items[15].Value - model.Items[13].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "20",
                        Value = model.ItemsCalcValues["170V2"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "21",
                        Value = model.ItemsCalcValues["170V2"] / model.ItemsCalcValues["300V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "22",
                        Value = model.ItemsCalcValues["170V1"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "23",
                        Value = model.ItemsCalcValues["170V1"] / model.ItemsCalcValues["300V1"] * 100
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "24",
                        Value = model.Items[20].Value - model.Items[18].Value
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "25",
                        Value = model.Items[21].Value - model.Items[19].Value
                    });

                    #endregion

                    #region Fill table 2

                    model2.ItemsCalcValues = new Dictionary<string, decimal?>
                    {
                        {"210V1", rows.FirstOrDefault(x => x.CodeItem == "210")?.Value1},
                        {"210V2", rows.FirstOrDefault(x => x.CodeItem == "210")?.Value2},
                        {"250V1", rows.FirstOrDefault(x => x.CodeItem == "250")?.Value1},
                        {"250V2", rows.FirstOrDefault(x => x.CodeItem == "250")?.Value2},
                        {"270V1", rows.FirstOrDefault(x => x.CodeItem == "270")?.Value1},
                        {"270V2", rows.FirstOrDefault(x => x.CodeItem == "270")?.Value2},
                        {"290V1", rows.FirstOrDefault(x => x.CodeItem == "290")?.Value1},
                        {"290V2", rows.FirstOrDefault(x => x.CodeItem == "290")?.Value2},
                        {"300V1", rows.FirstOrDefault(x => x.CodeItem == "300")?.Value1},
                        {"300V2", rows.FirstOrDefault(x => x.CodeItem == "300")?.Value2},
                    };

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "2",
                        Value = model2.ItemsCalcValues["290V2"]
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "3",
                        Value = model2.ItemsCalcValues["290V2"] / model2.ItemsCalcValues["300V2"] * 100
                    });

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "4",
                        Value = model2.ItemsCalcValues["290V1"]
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "5",
                        Value = model2.ItemsCalcValues["290V1"] / model2.ItemsCalcValues["300V1"] * 100
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "6",
                        Value = model2.Items[2].Value - model2.Items[0].Value
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "7",
                        Value = model2.Items[3].Value - model2.Items[1].Value
                    });

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "8",
                        Value = model2.ItemsCalcValues["210V2"]
                    });

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "9",
                        Value = model2.ItemsCalcValues["210V2"] / model2.ItemsCalcValues["300V2"] * 100
                    });

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "10",
                        Value = model2.ItemsCalcValues["210V1"]
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "11",
                        Value = model2.ItemsCalcValues["210V1"] / model2.ItemsCalcValues["300V1"] * 100
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "12",
                        Value = model2.Items[8].Value - model2.Items[6].Value
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "13",
                        Value = model2.Items[9].Value - model2.Items[7].Value
                    });

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "14",
                        Value = model2.ItemsCalcValues["250V2"]
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "15",
                        Value = model2.ItemsCalcValues["250V2"] / model2.ItemsCalcValues["300V2"] * 100
                    });

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "16",
                        Value = model2.ItemsCalcValues["250V1"]
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "17",
                        Value = model2.ItemsCalcValues["250V1"] / model2.ItemsCalcValues["300V1"] * 100
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "18",
                        Value = model2.Items[14].Value - model2.Items[12].Value
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "19",
                        Value = model2.Items[15].Value - model2.Items[13].Value
                    });

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "20",
                        Value = model2.ItemsCalcValues["270V2"]
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "21",
                        Value = model2.ItemsCalcValues["270V2"] / model2.ItemsCalcValues["300V2"] * 100
                    });

                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "22",
                        Value = model2.ItemsCalcValues["270V1"]
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "23",
                        Value = model2.ItemsCalcValues["270V1"] / model2.ItemsCalcValues["300V1"] * 100
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "24",
                        Value = model2.Items[20].Value - model2.Items[18].Value
                    });
                    model2.Items.Add(new AnalyticsItemModel
                    {
                        Index = "25",
                        Value = model2.Items[21].Value - model2.Items[19].Value
                    });

                    #endregion
                }

                analyticsTable1.Add(model);
                analyticsTable2.Add(model2);
            }

            #region Group and sort

            var internalAnalyticsTable1 =
                analyticsTable1.Where(x => validReportsIds.Contains(x.InternalReport.Id))
                    .Select(x => (AnalyticsModel) x).ToArray();
            var internalAnalyticsTable2 =
                analyticsTable2.Where(x => validReportsIds.Contains(x.InternalReport.Id))
                    .Select(x => (AnalyticsModel) x).ToArray();

            var indSort = new List<AnalyticsModel>();
            var unIndSort = new List<AnalyticsModel>();
            var calcColumns = new Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>>
            {
                {"3", decimals => decimals["190V2"] / decimals["300V2"] * 100},
                {"5", decimals => decimals["190V1"] / decimals["300V1"] * 100},
                {"6", decimals => decimals["190V1"] - decimals["190V2"]},
                {
                    "7",
                    decimals => decimals["190V1"] / decimals["300V1"] * 100 -
                                decimals["190V2"] / decimals["300V2"] * 100
                },
                {"9", decimals => decimals["110V2"] / decimals["300V2"] * 100},
                {"11", decimals => decimals["110V1"] / decimals["300V1"] * 100},
                {"12", decimals => decimals["110V1"] - decimals["110V2"]},
                {
                    "13",
                    decimals => decimals["110V1"] / decimals["300V1"] * 100 -
                                decimals["110V2"] / decimals["300V2"] * 100
                },
                {"15", decimals => decimals["150V2"] / decimals["300V2"] * 100},
                {"17", decimals => decimals["150V1"] / decimals["300V1"] * 100},
                {"18", decimals => decimals["150V1"] - decimals["150V2"]},
                {
                    "19",
                    decimals => decimals["150V1"] / decimals["300V1"] * 100 -
                                decimals["150V2"] / decimals["300V2"] * 100
                },
                {"21", decimals => decimals["170V2"] / decimals["300V2"] * 100},
                {"23", decimals => decimals["170V1"] / decimals["300V1"] * 100},
                {"24", decimals => decimals["170V1"] - decimals["170V2"]},
                {
                    "25",
                    decimals => decimals["170V1"] / decimals["300V1"] * 100 -
                                decimals["170V2"] / decimals["300V2"] * 100
                },
            };

            var calcColumns1 = new Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>>
            {
                {"3", decimals => decimals["290V2"] / decimals["300V2"] * 100},
                {"5", decimals => decimals["290V1"] / decimals["300V1"] * 100},
                {"6", decimals => decimals["290V1"] - decimals["290V2"]},
                {
                    "7",
                    decimals => decimals["290V1"] / decimals["300V1"] * 100 -
                                decimals["290V2"] / decimals["300V2"] * 100
                },
                {"9", decimals => decimals["210V2"] / decimals["300V2"] * 100},
                {"11", decimals => decimals["210V1"] / decimals["300V1"] * 100},
                {"12", decimals => decimals["210V1"] - decimals["210V2"]},
                {
                    "13",
                    decimals => decimals["210V1"] / decimals["300V1"] * 100 -
                                decimals["210V2"] / decimals["300V2"] * 100
                },
                {"15", decimals => decimals["250V2"] / decimals["300V2"] * 100},
                {"17", decimals => decimals["250V1"] / decimals["300V1"] * 100},
                {"18", decimals => decimals["250V1"] - decimals["250V2"]},
                {
                    "19",
                    decimals => decimals["250V1"] / decimals["300V1"] * 100 -
                                decimals["250V2"] / decimals["300V2"] * 100
                },
                {"21", decimals => decimals["270V2"] / decimals["300V2"] * 100},
                {"23", decimals => decimals["270V1"] / decimals["300V1"] * 100},
                {"24", decimals => decimals["270V1"] - decimals["270V2"]},
                {
                    "25",
                    decimals => decimals["270V1"] / decimals["300V1"] * 100 -
                                decimals["270V2"] / decimals["300V2"] * 100
                },
            };
            if (internalAnalyticsTable1.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                //is industrial
                var firstInd = internalAnalyticsTable1
                    .Where(x => x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newInd = GroupBy(calcColumns, firstInd, true, "ИТОГО ПРОМЫШЛЕННЫЕ");
                indSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.Industrial)
                    ? newInd
                    : newInd.GetRange(0, newInd.Count - 1));
            }

            if (internalAnalyticsTable1.Any(x => !x.Organization.TypeActivity.IsIndustrial))
            {
                //is unIndustrial
                var firstUnInd = internalAnalyticsTable1
                    .Where(x => !x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newUnInd = GroupBy(calcColumns, firstUnInd, false, "ИТОГО НЕПРОМЫШЛЕННЫЕ");
                unIndSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.UnIndustrial)
                    ? newUnInd
                    : newUnInd.GetRange(0, newUnInd.Count - 1));
            }

            var indSort2 = new List<AnalyticsModel>();
            var unIndSort2 = new List<AnalyticsModel>();
            if (internalAnalyticsTable2.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                //is industrial
                var firstInd2 = internalAnalyticsTable2
                    .Where(x => x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newInd2 = GroupBy(calcColumns1, firstInd2, true, "ИТОГО ПРОМЫШЛЕННЫЕ");
                indSort2.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.Industrial)
                    ? newInd2
                    : newInd2.GetRange(0, newInd2.Count - 1));
            }

            if (internalAnalyticsTable2.Any(x => !x.Organization.TypeActivity.IsIndustrial))
            {
                //is unIndustrial
                var firstUnInd2 = internalAnalyticsTable2
                    .Where(x => !x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newUnInd2 = GroupBy(calcColumns1, firstUnInd2, false, "ИТОГО НЕПРОМЫШЛЕННЫЕ");
                unIndSort2.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.UnIndustrial)
                    ? newUnInd2
                    : newUnInd2.GetRange(0, newUnInd2.Count - 1));
            }

            #endregion

            var tab1 = new List<AnalyticsModel>();
            var tab2 = new List<AnalyticsModel>();
            tab1.AddRange(indSort);
            tab1.AddRange(unIndSort);
            if (internalAnalyticsTable1.Any(x => !x.Organization.TypeActivity.IsIndustrial) &&
                internalAnalyticsTable1.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                if (tab1.Any(x => x.TitleRow.Contains("ИТОГО")))
                {
                    var (item1, item2) = GetSum(tab1.Where(x => x.TitleRow.Contains("ИТОГО")).ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }


                if (tab1.Any(x =>
                    x.Organization != null &&
                    x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase)))
                {
                    var (item1, item2) = GetSum(
                        tab1.Where(x =>
                                x.Organization != null &&
                                x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase))
                            .ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО без концерна",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }
            }

            tab2.AddRange(indSort2);
            tab2.AddRange(unIndSort2);

            if (internalAnalyticsTable1.Any(x => !x.Organization.TypeActivity.IsIndustrial) &&
                internalAnalyticsTable1.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                if (tab2.Any(x => x.TitleRow.Contains("ИТОГО")))
                {
                    var (item1, item2) = GetSum(tab2.Where(x => x.TitleRow.Contains("ИТОГО")).ToList(), calcColumns1);
                    // total with koncern
                    tab2.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }

                if (tab2.Any(x =>
                    x.Organization != null &&
                    x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase)))
                {
                    var (item1, item2) = GetSum(
                        tab2.Where(x =>
                                x.Organization != null &&
                                x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase))
                            .ToList(), calcColumns1);
                    // total with koncern
                    tab2.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО без концерна",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }
            }

            if (internalAnalyticsTable1.Any(x => x.Organization != null && x.Organization.IsHolding))
            {
                var (item1, item2) = GetSum(
                    analyticsTable1.Where(x => x.Organization != null && x.Organization.IsHolding)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Итого холдинг Д/О организаций",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable2.Any(x => x.Organization != null && x.Organization.IsHolding))
            {
                var (item1, item2) = GetSum(analyticsTable2
                        .Where(x => x.Organization != null && x.Organization.IsHolding)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns1);
                tab2.Add(new AnalyticsModel
                {
                    TitleRow = "Итого холдинг Д/О организаций",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable1.Any(x => x.Organization != null && x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable1.Where(x => x.Organization != null && x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей 50% и более",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable2.Any(x => x.Organization != null && x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable2.Where(x => x.Organization != null && x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns1);
                tab2.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей 50% и более",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable1.Any(x => x.Organization != null && !x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable1
                        .Where(x => x.Organization != null && !x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей менее 50%",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable2.Any(x => x.Organization != null && !x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable2
                        .Where(x => x.Organization != null && !x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns1);
                tab2.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей менее 50%",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            return new List<List<Dictionary<string, string>>>
            {
                tab1.Where(x => x.Items != null)
                    .Select(x => x.ToDictionary(tab1.Max(y => y.Items.Count), isExcelExport: isExport)).ToList(),
                tab2.Where(x => x.Items != null)
                    .Select(x => x.ToDictionary(tab2.Max(y => y.Items.Count), isExcelExport: isExport)).ToList(),
            };
        }

        public async Task<List<List<Dictionary<string, string>>>> GetStructureObligations(
            ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var reportObjets = (dynamic) await _reportRepository.GetReportStatistics(filterModel);
            var reports = (Report[]) reportObjets.validReports;
            var validReportsIds = await _reportRepository.GetReportIdByMultiFilter(filterModel);

            var analyticsTable = new List<AnalyticsWithReportModel>();
            foreach (var report in reports)
            {
                var model = new AnalyticsWithReportModel
                {
                    Organization = report.UserCheckinInterval.Organization,
                    Items = new List<AnalyticsItemModel>(),
                    TitleRow = report.UserCheckinInterval.Organization.Name,
                    InternalReport = report
                };
                var fullReport = await GetFullReport(report.Id);
                var tabModel = fullReport.TabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab);
                if (tabModel != null)
                {
                    var rows = MyConverter.ConvertToClass<BalanceSheetItem>(tabModel.Table.Rows).ToList();

                    #region Fill table 1

                    model.ItemsCalcValues = new Dictionary<string, decimal?>()
                    {
                        {"490V1", rows.FirstOrDefault(x => x.CodeItem == "490")?.Value1},
                        {"490V2", rows.FirstOrDefault(x => x.CodeItem == "490")?.Value2},
                        {"590V1", rows.FirstOrDefault(x => x.CodeItem == "590")?.Value1},
                        {"590V2", rows.FirstOrDefault(x => x.CodeItem == "590")?.Value2},
                        {"690V1", rows.FirstOrDefault(x => x.CodeItem == "690")?.Value1},
                        {"690V2", rows.FirstOrDefault(x => x.CodeItem == "690")?.Value2},
                        {"700V1", rows.FirstOrDefault(x => x.CodeItem == "700")?.Value1},
                        {"700V2", rows.FirstOrDefault(x => x.CodeItem == "700")?.Value2}
                    };

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "2",
                        Value = model.ItemsCalcValues["490V2"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "3",
                        Value = model.ItemsCalcValues["490V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "4",
                        Value = model.Items[1].Value - model.Items[0].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "5",
                        Value = model.ItemsCalcValues["590V2"] + model.ItemsCalcValues["690V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "6",
                        Value = model.ItemsCalcValues["590V1"] + model.ItemsCalcValues["690V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "7",
                        Value = model.Items[4].Value - model.Items[3].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "8",
                        Value = model.Items[0].Value + model.Items[3].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "9",
                        Value = model.Items[1].Value + model.Items[4].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "10",
                        Value = model.Items[7].Value - model.Items[6].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "11",
                        Value = model.ItemsCalcValues["490V2"] / model.ItemsCalcValues["700V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "12",
                        Value = model.ItemsCalcValues["490V1"] / model.ItemsCalcValues["700V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "13",
                        Value = model.Items[10].Value - model.Items[9].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "14",
                        Value = (model.ItemsCalcValues["590V2"] + model.ItemsCalcValues["690V2"]) /
                                model.ItemsCalcValues["700V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "15",
                        Value = (model.ItemsCalcValues["590V1"] + model.ItemsCalcValues["690V1"]) /
                                model.ItemsCalcValues["700V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "16",
                        Value = model.Items[13].Value - model.Items[12].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "17",
                        Value = (model.ItemsCalcValues["590V2"] + model.ItemsCalcValues["690V2"]) /
                                model.ItemsCalcValues["490V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "18",
                        Value = (model.ItemsCalcValues["590V1"] + model.ItemsCalcValues["690V1"]) /
                                model.ItemsCalcValues["490V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "19",
                        Value = model.Items[16].Value - model.Items[15].Value
                    });

                    #endregion
                }

                analyticsTable.Add(model);
            }

            var internalAnalyticsTable = analyticsTable.Where(x => validReportsIds.Contains((x.InternalReport.Id)))
                .Select(x => (AnalyticsModel) x).ToArray();

            var indSort = new List<AnalyticsModel>();
            var unIndSort = new List<AnalyticsModel>();
            var calcColumns = new Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>>
            {
                {"4", decimals => decimals["490V1"] - decimals["490V2"]},
                {"5", decimals => decimals["590V2"] + decimals["690V2"]},
                {"6", decimals => decimals["590V1"] + decimals["690V1"]},
                {
                    "7",
                    decimals => decimals["590V1"] + decimals["690V1"] -
                                (decimals["590V2"] + decimals["690V2"])
                },
                {
                    "8",
                    decimals => decimals["490V2"] + decimals["590V2"] +
                                decimals["690V2"]
                },
                {
                    "9",
                    decimals => decimals["490V1"] + decimals["590V1"] +
                                decimals["690V1"]
                },
                {
                    "10",
                    decimals => decimals["490V1"] + decimals["590V1"] +
                        decimals["690V1"] - (decimals["490V2"] +
                                             decimals["590V2"] +
                                             decimals["690V2"])
                },
                {"11", decimals => decimals["490V2"] / decimals["700V2"]},
                {"12", decimals => decimals["490V1"] / decimals["700V1"]},
                {
                    "13",
                    decimals => decimals["490V1"] / decimals["700V1"] -
                                decimals["490V2"] / decimals["700V2"]
                },
                {
                    "14",
                    decimals => (decimals["590V2"] + decimals["690V2"]) /
                                decimals["700V2"]
                },
                {
                    "15",
                    decimals => (decimals["590V1"] + decimals["690V1"]) /
                                decimals["700V1"]
                },
                {
                    "16",
                    decimals =>
                        (decimals["590V1"] + decimals["690V1"]) /
                        decimals["700V1"] -
                        (decimals["590V2"] + decimals["690V2"]) /
                        decimals["700V2"]
                },
                {
                    "17",
                    decimals => (decimals["590V2"] + decimals["690V2"]) /
                                decimals["490V2"]
                },
                {
                    "18",
                    decimals => (decimals["590V1"] + decimals["690V1"]) /
                                decimals["490V1"]
                },
                {
                    "19",
                    decimals =>
                        (decimals["590V1"] + decimals["690V1"]) /
                        decimals["490V1"] -
                        (decimals["590V2"] + decimals["690V2"]) /
                        decimals["490V2"]
                },
            };
            if (internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                //is industrial
                var firstInd = internalAnalyticsTable
                    .Where(x => x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newInd = GroupBy(calcColumns, firstInd, true, "ИТОГО ПРОМЫШЛЕННЫЕ");
                indSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.Industrial)
                    ? newInd
                    : newInd.GetRange(0, newInd.Count - 1));
            }

            if (internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial))
            {
                //is unIndustrial
                var firstUnInd = internalAnalyticsTable
                    .Where(x => !x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newUnInd = GroupBy(calcColumns, firstUnInd, false, "ИТОГО НЕПРОМЫШЛЕННЫЕ");
                unIndSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.UnIndustrial)
                    ? newUnInd
                    : newUnInd.GetRange(0, newUnInd.Count - 1));
            }

            var tab1 = new List<AnalyticsModel>();
            tab1.AddRange(indSort);
            tab1.AddRange(unIndSort);
            if (internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial) &&
                internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                if (tab1.Any(x => x.TitleRow.Contains("ИТОГО")))
                {
                    var (item1, item2) = GetSum(tab1.Where(x => x.TitleRow.Contains("ИТОГО")).ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }

                if (tab1.Any(x =>
                    x.Organization != null &&
                    x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase)))
                {
                    var (item1, item2) = GetSum(
                        tab1.Where(x =>
                                x.Organization != null &&
                                x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase))
                            .ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО без концерна",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsHolding))
            {
                var (item1, item2) = GetSum(
                    analyticsTable.Where(x => x.Organization != null && x.Organization.IsHolding)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Итого холдинг Д/О организаций",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей 50% и более",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && !x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && !x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей менее 50%",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            return new List<List<Dictionary<string, string>>>
            {
                tab1.Where(x => x.Items != null).Select(x =>
                    x.ToDictionary(maxItemCount: tab1.Max(y => y.Items.Count), isExcelExport: isExport)).ToList(),
            };
        }

        public async Task<List<List<Dictionary<string, string>>>> GetStatusOfOwnWorkingCapital(
            ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var reportObjets = (dynamic) await _reportRepository.GetReportStatistics(filterModel);
            var reports = (Report[]) reportObjets.validReports;
            var validReportsIds = await _reportRepository.GetReportIdByMultiFilter(filterModel);

            var analyticsTable = new List<AnalyticsWithReportModel>();
            foreach (var report in reports)
            {
                var model = new AnalyticsWithReportModel
                {
                    Organization = report.UserCheckinInterval.Organization,
                    Items = new List<AnalyticsItemModel>(),
                    TitleRow = report.UserCheckinInterval.Organization.Name,
                    InternalReport = report
                };
                var fullReport = await GetFullReport(report.Id);
                var tabModel = fullReport.TabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab);
                if (tabModel != null)
                {
                    var rows = MyConverter.ConvertToClass<BalanceSheetItem>(tabModel.Table.Rows).ToList();

                    #region Fill table 1

                    model.ItemsCalcValues = new Dictionary<string, decimal?>()
                    {
                        {"190V1", rows.FirstOrDefault(x => x.CodeItem == "190")?.Value1},

                        {"290V1", rows.FirstOrDefault(x => x.CodeItem == "290")?.Value1},

                        {"490V1", rows.FirstOrDefault(x => x.CodeItem == "490")?.Value1},

                        {"590V1", rows.FirstOrDefault(x => x.CodeItem == "590")?.Value1},

                        {
                            "#2", report.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                      "деревообрабатывающие предприятия",
                                      StringComparison.OrdinalIgnoreCase) ||
                                  report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("предприятия цбп",
                                      StringComparison.OrdinalIgnoreCase) ||
                                  report.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                      "лесозаготовительные организации",
                                      StringComparison.OrdinalIgnoreCase) ? new decimal(0.2) :
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("мебельные предприятия",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(0.3) :
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("торговые предприятия",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(0.1) : 0
                        }
                    };

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "2",
                        Value = model.ItemsCalcValues["#2"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "3",
                        Value = model.ItemsCalcValues["490V1"] + model.ItemsCalcValues["590V1"] -
                                model.ItemsCalcValues["190V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "4",
                        Value = model.ItemsCalcValues["290V1"] * model.Items[0].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "5",
                        Value = model.Items[2].Value > model.Items[1].Value
                            ? model.Items[2].Value - model.Items[1].Value
                            : null
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "6",
                        Value = model.Items[2].Value < model.Items[1].Value
                            ? model.Items[1].Value - model.Items[2].Value
                            : null
                    });

                    #endregion
                }

                analyticsTable.Add(model);
            }

            var internalAnalyticsTable = analyticsTable.Where(x => validReportsIds.Contains((x.InternalReport.Id)))
                .Select(x => (AnalyticsModel) x).ToArray();

            var indSort = new List<AnalyticsModel>();
            var unIndSort = new List<AnalyticsModel>();
            var calcColumns = new Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>>
            {
                {
                    "3",
                    decimals => decimals["490V1"] + decimals["590V1"] - decimals["190V1"]
                },
                {
                    "4",
                    decimals => decimals["290V1"] * decimals["#2"]
                },
                {
                    "5",
                    decimals =>
                        decimals["290V1"] * decimals["#2"] > decimals["490V1"] + decimals["590V1"] - decimals["190V1"]
                            ? decimals["290V1"] * decimals["#2"] -
                              (decimals["490V1"] + decimals["590V1"] - decimals["190V1"])
                            : null
                },
                {
                    "6",
                    decimals =>
                        decimals["290V1"] * decimals["#2"] < decimals["490V1"] + decimals["590V1"] - decimals["190V1"]
                            ? decimals["490V1"] + decimals["590V1"] - decimals["190V1"] -
                              decimals["290V1"] * decimals["#2"]
                            : null
                },
            };
            if (internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                //is industrial
                var firstInd = internalAnalyticsTable
                    .Where(x => x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newInd = GroupBy(calcColumns, firstInd, true, "ИТОГО ПРОМЫШЛЕННЫЕ", new[] {"2", "4", "5", "6"},
                    new[] {"#2", "2"});
                indSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.Industrial)
                    ? newInd
                    : newInd.GetRange(0, newInd.Count - 1));
            }

            if (internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial))
            {
                //is unIndustrial
                var firstUnInd = internalAnalyticsTable
                    .Where(x => !x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newUnInd = GroupBy(calcColumns, firstUnInd, false, "ИТОГО НЕПРОМЫШЛЕННЫЕ",
                    new[] {"2", "4", "5", "6"}, new[] {"#2", "2"});
                unIndSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.UnIndustrial)
                    ? newUnInd
                    : newUnInd.GetRange(0, newUnInd.Count - 1));
            }

            var tab1 = new List<AnalyticsModel>();
            tab1.AddRange(indSort);
            tab1.AddRange(unIndSort);
            if (new[] {ReportStatisticsFilterEnum.Industrial, ReportStatisticsFilterEnum.UnIndustrial}
                    .All(x => filterModel.Filter.Contains(x)) &&
                internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial) &&
                internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                if (tab1.Any(x => x.TitleRow.Contains("ИТОГО")))
                {
                    var (item1, item2) = GetSum(tab1.Where(x => x.TitleRow.Contains("ИТОГО")).ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО",
                        Items = item1.Select(x =>
                            (bool) new[] {"2", "4", "5", "6"}?.Contains(x.Index)
                                ? new AnalyticsItemModel {Index = x.Index, Value = null}
                                : x).ToList(),
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }

                if (tab1.Any(x =>
                    x.Organization != null &&
                    x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase)))
                {
                    var (item1, item2) = GetSum(
                        tab1.Where(x =>
                                x.Organization != null &&
                                x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase))
                            .ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО без концерна",
                        Items = item1.Select(x =>
                            (bool) new[] {"2", "4", "5", "6"}?.Contains(x.Index)
                                ? new AnalyticsItemModel {Index = x.Index, Value = null}
                                : x).ToList(),
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsHolding))
            {
                var (item1, item2) = GetSum(
                    analyticsTable.Where(x => x.Organization != null && x.Organization.IsHolding)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Итого холдинг Д/О организаций",
                    Items = item1.Select(x =>
                        (bool) new[] {"2", "4", "5", "6"}?.Contains(x.Index)
                            ? new AnalyticsItemModel {Index = x.Index, Value = null}
                            : x).ToList(),
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей 50% и более",
                    Items = item1.Select(x =>
                        (bool) new[] {"2", "4", "5", "6"}?.Contains(x.Index)
                            ? new AnalyticsItemModel {Index = x.Index, Value = null}
                            : x).ToList(),
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && !x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && !x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей менее 50%",
                    Items = item1.Select(x =>
                        (bool) new[] {"2", "4", "5", "6"}?.Contains(x.Index)
                            ? new AnalyticsItemModel {Index = x.Index, Value = null}
                            : x).ToList(),
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            return new List<List<Dictionary<string, string>>>
            {
                tab1.Where(x => x.Items != null).Select(x =>
                    x.ToDictionary(maxItemCount: tab1.Max(y => y.Items.Count), isExcelExport: isExport)).ToList(),
            };
        }

        public async Task<List<List<Dictionary<string, string>>>> GetSolvencyRatios(
            ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var reportObjets = (dynamic) await _reportRepository.GetReportStatistics(filterModel);
            var reports = (Report[]) reportObjets.validReports;
            var validReportsIds = await _reportRepository.GetReportIdByMultiFilter(filterModel);

            var analyticsTable = new List<AnalyticsWithReportModel>();
            foreach (var report in reports)
            {
                var model = new AnalyticsWithReportModel
                {
                    Organization = report.UserCheckinInterval.Organization,
                    Items = new List<AnalyticsItemModel>(),
                    TitleRow = report.UserCheckinInterval.Organization.Name,
                    InternalReport = report
                };
                var fullReport = await GetFullReport(report.Id);
                var tabModel = fullReport.TabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab);
                if (tabModel != null)
                {
                    var rows = MyConverter.ConvertToClass<BalanceSheetItem>(tabModel.Table.Rows).ToList();

                    #region Fill table 1

                    model.ItemsCalcValues = new Dictionary<string, decimal?>()
                    {
                        {"190V2", rows.FirstOrDefault(x => x.CodeItem == "190")?.Value2},
                        {"190V1", rows.FirstOrDefault(x => x.CodeItem == "190")?.Value1},

                        {"290V2", rows.FirstOrDefault(x => x.CodeItem == "290")?.Value2},
                        {"290V1", rows.FirstOrDefault(x => x.CodeItem == "290")?.Value1},

                        {"300V2", rows.FirstOrDefault(x => x.CodeItem == "300")?.Value2},
                        {"300V1", rows.FirstOrDefault(x => x.CodeItem == "300")?.Value1},

                        {"490V2", rows.FirstOrDefault(x => x.CodeItem == "490")?.Value2},
                        {"490V1", rows.FirstOrDefault(x => x.CodeItem == "490")?.Value1},

                        {"590V2", rows.FirstOrDefault(x => x.CodeItem == "590")?.Value2},
                        {"590V1", rows.FirstOrDefault(x => x.CodeItem == "590")?.Value1},

                        {"690V2", rows.FirstOrDefault(x => x.CodeItem == "690")?.Value2},
                        {"690V1", rows.FirstOrDefault(x => x.CodeItem == "690")?.Value1},
                    };

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "2",
                        Value =
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "деревообрабатывающие предприятия", StringComparison.OrdinalIgnoreCase) ||
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("предприятия цбп",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(1.3) :
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("мебельные предприятия",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(1.7) :
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "лесозаготовительные организации",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(1.5) :
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("торговые предприятия",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(1.0) :
                            report.UserCheckinInterval.Organization.Name.Contains("экспрессоснова",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(1.15) : 0
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "3",
                        Value = model.ItemsCalcValues["290V2"] / model.ItemsCalcValues["690V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "4",
                        Value = model.ItemsCalcValues["290V1"] / model.ItemsCalcValues["690V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "5",
                        Value = model.Items[2].Value - model.Items[1].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "6",
                        Value = report.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                    "деревообрабатывающие предприятия", StringComparison.OrdinalIgnoreCase) ||
                                report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("предприятия цбп",
                                    StringComparison.OrdinalIgnoreCase) ? new decimal(0.2) :
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("мебельные предприятия",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(0.3) :
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "лесозаготовительные организации",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(0.2) :
                            report.UserCheckinInterval.Organization.TypeActivity.Name.Contains("торговые предприятия",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(0.1) :
                            report.UserCheckinInterval.Organization.Name.Contains("экспрессоснова",
                                StringComparison.OrdinalIgnoreCase) ? new decimal(0.15) : 0
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "7",
                        Value = (model.ItemsCalcValues["490V2"] + model.ItemsCalcValues["590V2"] -
                                 model.ItemsCalcValues["190V2"]) / model.ItemsCalcValues["290V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "8",
                        Value = (model.ItemsCalcValues["490V1"] + model.ItemsCalcValues["590V1"] -
                                 model.ItemsCalcValues["190V1"]) / model.ItemsCalcValues["290V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "9",
                        Value = model.Items[6].Value - model.Items[5].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "10",
                        Value = (model.ItemsCalcValues["590V2"] + model.ItemsCalcValues["690V2"]) /
                                model.ItemsCalcValues["300V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "11",
                        Value = (model.ItemsCalcValues["590V1"] + model.ItemsCalcValues["690V1"]) /
                                model.ItemsCalcValues["300V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "12",
                        Value = model.Items[9].Value - model.Items[8].Value
                    });

                    #endregion
                }

                analyticsTable.Add(model);
            }

            var internalAnalyticsTable = analyticsTable.Where(x => validReportsIds.Contains((x.InternalReport.Id)))
                .Select(x => (AnalyticsModel) x).ToArray();

            var indSort = new List<AnalyticsModel>();
            var unIndSort = new List<AnalyticsModel>();
            var calcColumns = new Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>>
            {
                {
                    "3",
                    decimals => decimals["290V2"] / decimals["690V2"]
                },
                {
                    "4",
                    decimals => decimals["290V1"] / decimals["690V1"]
                },
                {
                    "5",
                    decimals => decimals["290V1"] / decimals["690V1"] - decimals["290V2"] / decimals["690V2"]
                },
                {
                    "7",
                    decimals => (decimals["490V2"] + decimals["590V2"] - decimals["190V2"]) / decimals["290V2"]
                },
                {
                    "8",
                    decimals => (decimals["490V1"] + decimals["590V1"] - decimals["190V1"]) / decimals["290V1"]
                },
                {
                    "9",
                    decimals => (decimals["490V1"] + decimals["590V1"] - decimals["190V1"]) / decimals["290V1"] -
                                (decimals["490V2"] + decimals["590V2"] - decimals["190V2"]) / decimals["290V2"]
                },
                {
                    "10",
                    decimals => (decimals["590V2"] + decimals["690V2"]) / decimals["300V2"]
                },
                {
                    "11",
                    decimals => (decimals["590V1"] + decimals["690V1"]) / decimals["300V1"]
                },
                {
                    "12",
                    decimals => (decimals["590V1"] + decimals["690V1"]) / decimals["300V1"] -
                                (decimals["590V2"] + decimals["690V2"]) / decimals["300V2"]
                }
            };
            if (internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                //is industrial
                var firstInd = internalAnalyticsTable
                    .Where(x => x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newInd = GroupBy(calcColumns, firstInd, true, "ИТОГО ПРОМЫШЛЕННЫЕ", new[] {"2", "6"},
                    new[] {"2", "6"});
                indSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.Industrial)
                    ? newInd
                    : newInd.GetRange(0, newInd.Count - 1));
            }

            if (internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial))
            {
                //is unIndustrial
                var firstUnInd = internalAnalyticsTable
                    .Where(x => !x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newUnInd = GroupBy(calcColumns, firstUnInd, false, "ИТОГО НЕПРОМЫШЛЕННЫЕ", new[] {"2", "6"},
                    new[] {"2", "6"});
                unIndSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.UnIndustrial)
                    ? newUnInd
                    : newUnInd.GetRange(0, newUnInd.Count - 1));
            }

            var tab1 = new List<AnalyticsModel>();
            tab1.AddRange(indSort);
            tab1.AddRange(unIndSort);
            if (new[] {ReportStatisticsFilterEnum.Industrial, ReportStatisticsFilterEnum.UnIndustrial}
                    .All(x => filterModel.Filter.Contains(x)) &&
                internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial) &&
                internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                if (tab1.Any(x => x.TitleRow.Contains("ИТОГО")))
                {
                    var (item1, item2) = GetSum(tab1.Where(x => x.TitleRow.Contains("ИТОГО")).ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО",
                        Items = item1.Select(x =>
                            (bool) new[] {"2", "6"}?.Contains(x.Index)
                                ? new AnalyticsItemModel {Index = x.Index, Value = null}
                                : x).ToList(),
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }

                if (tab1.Any(x =>
                    x.Organization != null &&
                    x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase)))
                {
                    var (item1, item2) = GetSum(
                        tab1.Where(x =>
                                x.Organization != null &&
                                x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase))
                            .ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО без концерна",
                        Items = item1.Select(x =>
                            (bool) new[] {"2", "6"}?.Contains(x.Index)
                                ? new AnalyticsItemModel {Index = x.Index, Value = null}
                                : x).ToList(),
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsHolding))
            {
                var (item1, item2) = GetSum(
                    analyticsTable.Where(x => x.Organization != null && x.Organization.IsHolding)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Итого холдинг Д/О организаций",
                    Items = item1.Select(x =>
                        (bool) new[] {"2", "6"}?.Contains(x.Index)
                            ? new AnalyticsItemModel {Index = x.Index, Value = null}
                            : x).ToList(),
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей 50% и более",
                    Items = item1.Select(x =>
                        (bool) new[] {"2", "6"}?.Contains(x.Index)
                            ? new AnalyticsItemModel {Index = x.Index, Value = null}
                            : x).ToList(),
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && !x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && !x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей менее 50%",
                    Items = item1.Select(x =>
                        (bool) new[] {"2", "6"}?.Contains(x.Index)
                            ? new AnalyticsItemModel {Index = x.Index, Value = null}
                            : x).ToList(),
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            return new List<List<Dictionary<string, string>>>
            {
                tab1.Where(x => x.Items != null).Select(x => x.ToDictionary(tab1.Max(y => y.Items.Count), 3, false))
                    .ToList(),
            };
        }

        public async Task<List<List<Dictionary<string, string>>>> GetBalanceSheetLiabilitiesStructure(
            ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var reportObjets = (dynamic) await _reportRepository.GetReportStatistics(filterModel);
            var reports = (Report[]) reportObjets.validReports;
            var validReportsIds = await _reportRepository.GetReportIdByMultiFilter(filterModel);

            var analyticsTable = new List<AnalyticsWithReportModel>();
            foreach (var report in reports)
            {
                var model = new AnalyticsWithReportModel
                {
                    Organization = report.UserCheckinInterval.Organization,
                    Items = new List<AnalyticsItemModel>(),
                    TitleRow = report.UserCheckinInterval.Organization.Name,
                    InternalReport = report
                };
                var fullReport = await GetFullReport(report.Id);
                var tabModel = fullReport.TabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab);
                if (tabModel != null)
                {
                    var rows = MyConverter.ConvertToClass<BalanceSheetItem>(tabModel.Table.Rows).ToList();

                    #region Fill table 1

                    model.ItemsCalcValues = new Dictionary<string, decimal?>()
                    {
                        {"490V2", rows.FirstOrDefault(x => x.CodeItem == "490")?.Value2},
                        {"490V1", rows.FirstOrDefault(x => x.CodeItem == "490")?.Value1},

                        {"700V2", rows.FirstOrDefault(x => x.CodeItem == "700")?.Value2},
                        {"700V1", rows.FirstOrDefault(x => x.CodeItem == "700")?.Value1},

                        {"590V2", rows.FirstOrDefault(x => x.CodeItem == "590")?.Value2},
                        {"590V1", rows.FirstOrDefault(x => x.CodeItem == "590")?.Value1},

                        {"690V2", rows.FirstOrDefault(x => x.CodeItem == "690")?.Value2},
                        {"690V1", rows.FirstOrDefault(x => x.CodeItem == "690")?.Value1},

                        {"610V2", rows.FirstOrDefault(x => x.CodeItem == "610")?.Value2},
                        {"610V1", rows.FirstOrDefault(x => x.CodeItem == "610")?.Value1},

                        {"620V2", rows.FirstOrDefault(x => x.CodeItem == "620")?.Value2},
                        {"620V1", rows.FirstOrDefault(x => x.CodeItem == "620")?.Value1},

                        {"630V2", rows.FirstOrDefault(x => x.CodeItem == "630")?.Value2},
                        {"630V1", rows.FirstOrDefault(x => x.CodeItem == "620")?.Value1},
                    };

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "2",
                        Value = model.ItemsCalcValues["490V2"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "3",
                        Value = model.ItemsCalcValues["490V2"] / model.ItemsCalcValues["700V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "4",
                        Value = model.ItemsCalcValues["490V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "5",
                        Value = model.ItemsCalcValues["490V1"] / model.ItemsCalcValues["700V1"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "6",
                        Value = model.Items[2].Value - model.Items[0].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "7",
                        Value = model.Items[3].Value - model.Items[1].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "8",
                        Value = model.ItemsCalcValues["590V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "9",
                        Value = model.ItemsCalcValues["590V2"] / model.ItemsCalcValues["700V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "10",
                        Value = model.ItemsCalcValues["590V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "11",
                        Value = model.ItemsCalcValues["590V1"] / model.ItemsCalcValues["700V1"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "12",
                        Value = model.Items[8].Value - model.Items[6].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "13",
                        Value = model.Items[9].Value - model.Items[7].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "14",
                        Value = model.ItemsCalcValues["690V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "15",
                        Value = model.ItemsCalcValues["690V2"] / model.ItemsCalcValues["700V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "16",
                        Value = model.ItemsCalcValues["690V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "17",
                        Value = model.ItemsCalcValues["690V1"] / model.ItemsCalcValues["700V1"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "18",
                        Value = model.Items[14].Value - model.Items[12].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "19",
                        Value = model.Items[15].Value - model.Items[13].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "20",
                        Value = model.ItemsCalcValues["610V2"] + model.ItemsCalcValues["620V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "21",
                        Value = (model.ItemsCalcValues["610V2"] + model.ItemsCalcValues["620V2"]) /
                            model.ItemsCalcValues["700V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "22",
                        Value = model.ItemsCalcValues["610V1"] + model.ItemsCalcValues["620V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "23",
                        Value = (model.ItemsCalcValues["610V1"] + model.ItemsCalcValues["620V1"]) /
                            model.ItemsCalcValues["700V1"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "24",
                        Value = model.Items[20].Value - model.Items[18].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "25",
                        Value = model.Items[21].Value - model.Items[19].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "26",
                        Value = model.ItemsCalcValues["630V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "27",
                        Value = model.ItemsCalcValues["630V2"] / model.ItemsCalcValues["700V2"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "28",
                        Value = model.ItemsCalcValues["630V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "29",
                        Value = model.ItemsCalcValues["630V1"] / model.ItemsCalcValues["700V1"] * 100
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "30",
                        Value = model.Items[26].Value - model.Items[24].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "31",
                        Value = model.Items[27].Value - model.Items[25].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "32",
                        Value = model.ItemsCalcValues["700V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "33",
                        Value = model.ItemsCalcValues["700V1"]
                    });

                    #endregion
                }

                analyticsTable.Add(model);
            }

            var internalAnalyticsTable = analyticsTable.Where(x => validReportsIds.Contains((x.InternalReport.Id)))
                .Select(x => (AnalyticsModel) x).ToArray();

            var indSort = new List<AnalyticsModel>();
            var unIndSort = new List<AnalyticsModel>();
            var calcColumns = new Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>>
            {
                {
                    "3",
                    decimals => decimals["490V2"] / decimals["700V2"] * 100
                },
                {
                    "5",
                    decimals => decimals["490V1"] / decimals["700V1"] * 100
                },
                {
                    "6",
                    decimals => decimals["490V1"] - decimals["490V2"]
                },
                {
                    "7",
                    decimals => decimals["490V1"] / decimals["700V1"] * 100 -
                                decimals["490V2"] / decimals["700V2"] * 100
                },
                {
                    "9",
                    decimals => decimals["590V2"] / decimals["700V2"] * 100
                },
                {
                    "11",
                    decimals => decimals["590V1"] / decimals["700V1"] * 100
                },
                {
                    "12",
                    decimals => decimals["590V1"] - decimals["590V2"]
                },
                {
                    "13",
                    decimals => decimals["590V1"] / decimals["700V1"] * 100 -
                                decimals["590V2"] / decimals["700V2"] * 100
                },
                {
                    "15",
                    decimals => decimals["690V2"] / decimals["700V2"] * 100
                },
                {
                    "17",
                    decimals => decimals["690V1"] / decimals["700V1"] * 100
                },
                {
                    "18",
                    decimals => decimals["690V1"] - decimals["690V2"]
                },
                {
                    "19",
                    decimals => decimals["690V1"] / decimals["700V1"] * 100 -
                                decimals["690V2"] / decimals["700V2"] * 100
                },
                {
                    "20",
                    decimals => decimals["610V2"] + decimals["620V2"]
                },
                {
                    "21",
                    decimals => (decimals["610V2"] + decimals["620V2"]) / decimals["700V2"] * 100
                },
                {
                    "22",
                    decimals => decimals["610V1"] + decimals["620V1"]
                },
                {
                    "23",
                    decimals => (decimals["610V1"] + decimals["620V1"]) / decimals["700V1"] * 100
                },
                {
                    "24",
                    decimals => decimals["610V1"] + decimals["620V1"] - decimals["610V2"] + decimals["620V2"]
                },
                {
                    "25",
                    decimals => (decimals["610V1"] + decimals["620V1"]) / decimals["700V1"] * 100 -
                                (decimals["610V2"] + decimals["620V2"]) / decimals["700V2"] * 100
                },
                {
                    "27",
                    decimals => decimals["630V2"] / decimals["700V2"] * 100
                },
                {
                    "29",
                    decimals => decimals["630V1"] / decimals["700V1"] * 100
                },
                {
                    "30",
                    decimals => decimals["630V1"] - decimals["630V2"]
                },
                {
                    "31",
                    decimals => decimals["630V1"] / decimals["700V1"] * 100 -
                                decimals["630V2"] / decimals["700V2"] * 100
                }
            };
            if (internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                //is industrial
                var firstInd = internalAnalyticsTable
                    .Where(x => x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newInd = GroupBy(calcColumns, firstInd, true, "ИТОГО ПРОМЫШЛЕННЫЕ");
                indSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.Industrial)
                    ? newInd
                    : newInd.GetRange(0, newInd.Count - 1));
            }

            if (internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial))
            {
                //is unIndustrial
                var firstUnInd = internalAnalyticsTable
                    .Where(x => !x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newUnInd = GroupBy(calcColumns, firstUnInd, false, "ИТОГО НЕПРОМЫШЛЕННЫЕ");
                unIndSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.UnIndustrial)
                    ? newUnInd
                    : newUnInd.GetRange(0, newUnInd.Count - 1));
            }

            var tab1 = new List<AnalyticsModel>();
            tab1.AddRange(indSort);
            tab1.AddRange(unIndSort);
            if (new[] {ReportStatisticsFilterEnum.Industrial, ReportStatisticsFilterEnum.UnIndustrial}
                    .All(x => filterModel.Filter.Contains(x)) &&
                internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial) &&
                internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                if (tab1.Any(x => x.TitleRow.Contains("ИТОГО")))
                {
                    var (item1, item2) = GetSum(tab1.Where(x => x.TitleRow.Contains("ИТОГО")).ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }

                if (tab1.Any(x =>
                    x.Organization != null &&
                    x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase)))
                {
                    var (item1, item2) = GetSum(
                        tab1.Where(x =>
                                x.Organization != null &&
                                x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase))
                            .ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО без концерна",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsHolding))
            {
                var (item1, item2) = GetSum(
                    analyticsTable.Where(x => x.Organization != null && x.Organization.IsHolding)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Итого холдинг Д/О организаций",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей 50% и более",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && !x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && !x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей менее 50%",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            return new List<List<Dictionary<string, string>>>
            {
                tab1.Where(x => x.Items != null).Select(x =>
                    x.ToDictionary(maxItemCount: tab1.Max(y => y.Items.Count), isExcelExport: isExport)).ToList(),
            };
        }

        public async Task<List<List<Dictionary<string, string>>>> GetFinancialIndicators(
            ReportStatisticsFilterModel filterModel, bool isExport = false)
        {
            var reportObjets = (dynamic) await _reportRepository.GetReportStatistics(filterModel);
            var reports = (Report[]) reportObjets.validReports;
            var validReportsIds = await _reportRepository.GetReportIdByMultiFilter(filterModel);

            var analyticsTable = new List<AnalyticsWithReportModel>();
            foreach (var report in reports)
            {
                var model = new AnalyticsWithReportModel
                {
                    Organization = report.UserCheckinInterval.Organization,
                    Items = new List<AnalyticsItemModel>(),
                    TitleRow = report.UserCheckinInterval.Organization.Name,
                    InternalReport = report
                };
                var fullReport = await GetFullReport(report.Id);
                var tabModel = fullReport.TabModels.FirstOrDefault(x => x.TabName == TabName.BalanceTab);
                var tabModel1 = fullReport.TabModels.FirstOrDefault(x => x.TabName == TabName.ProfitLossTab);
                if (tabModel != null && tabModel1 != null)
                {
                    var rows = MyConverter.ConvertToClass<BalanceSheetItem>(tabModel.Table.Rows).ToList();
                    var rows1 = MyConverter.ConvertToClass<ProfitLossItem>(tabModel1.Table.Rows).ToList();

                    #region Fill table 1

                    model.ItemsCalcValues = new Dictionary<string, decimal?>()
                    {
                        {"010V2", rows1.FirstOrDefault(x => x.CodeItem == "010")?.Value2},
                        {"010V1", rows1.FirstOrDefault(x => x.CodeItem == "010")?.Value1},

                        {"020V2", rows1.FirstOrDefault(x => x.CodeItem == "020")?.Value2},
                        {"020V1", rows1.FirstOrDefault(x => x.CodeItem == "020")?.Value1},

                        {"040V2", rows1.FirstOrDefault(x => x.CodeItem == "040")?.Value2},
                        {"040V1", rows1.FirstOrDefault(x => x.CodeItem == "040")?.Value1},

                        {"050V2", rows1.FirstOrDefault(x => x.CodeItem == "050")?.Value2},
                        {"050V1", rows1.FirstOrDefault(x => x.CodeItem == "050")?.Value1},

                        {"060V2", rows1.FirstOrDefault(x => x.CodeItem == "060")?.Value2},
                        {"060V1", rows1.FirstOrDefault(x => x.CodeItem == "060")?.Value1},

                        {"150V2", rows.FirstOrDefault(x => x.CodeItem == "150")?.Value2},
                        {"150V1", rows.FirstOrDefault(x => x.CodeItem == "150")?.Value1},

                        {"210V2", rows.FirstOrDefault(x => x.CodeItem == "210")?.Value2},
                        {"210V1", rows.FirstOrDefault(x => x.CodeItem == "210")?.Value1},
                    };

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "2",
                        Value = model.ItemsCalcValues["010V1"]
                    });
                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "3",
                        Value = model.ItemsCalcValues["010V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "4",
                        Value = model.Items[0].Value - model.Items[1].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "5",
                        Value = model.ItemsCalcValues["020V1"] + model.ItemsCalcValues["040V1"] +
                                model.ItemsCalcValues["050V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "6",
                        Value = model.ItemsCalcValues["020V2"] + model.ItemsCalcValues["040V2"] +
                                model.ItemsCalcValues["050V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "7",
                        Value = model.Items[3].Value - model.Items[4].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "8",
                        Value = (model.ItemsCalcValues["020V1"] + model.ItemsCalcValues["040V1"] +
                                 model.ItemsCalcValues["050V1"]) / model.ItemsCalcValues["010V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "9",
                        Value = (model.ItemsCalcValues["020V2"] + model.ItemsCalcValues["040V2"] +
                                 model.ItemsCalcValues["050V2"]) / model.ItemsCalcValues["010V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "10",
                        Value = model.Items[6].Value - model.Items[7].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "11",
                        Value = model.ItemsCalcValues["060V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "12",
                        Value = model.ItemsCalcValues["060V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "13",
                        Value = model.Items[9].Value - model.Items[10].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "14",
                        Value = model.ItemsCalcValues["150V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "15",
                        Value = model.ItemsCalcValues["150V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "16",
                        Value = model.Items[12].Value - model.Items[13].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "17",
                        Value = model.ItemsCalcValues["210V1"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "18",
                        Value = model.ItemsCalcValues["210V2"]
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "19",
                        Value = model.Items[15].Value - model.Items[16].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "20",
                        Value = model.Items[9].Value / model.Items[0].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "21",
                        Value = model.Items[10].Value / model.Items[1].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "22",
                        Value = model.Items[13].Value - model.Items[12].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "23",
                        Value = model.Items[9].Value / model.Items[3].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "24",
                        Value = model.Items[10].Value / model.Items[4].Value
                    });

                    model.Items.Add(new AnalyticsItemModel
                    {
                        Index = "25",
                        Value = model.Items[10].Value - model.Items[9].Value
                    });

                    #endregion
                }

                analyticsTable.Add(model);
            }

            var internalAnalyticsTable = analyticsTable.Where(x => validReportsIds.Contains((x.InternalReport.Id)))
                .Select(x => (AnalyticsModel) x).ToArray();

            var indSort = new List<AnalyticsModel>();
            var unIndSort = new List<AnalyticsModel>();
            var calcColumns = new Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>>
            {
                {
                    "4",
                    decimals => decimals["010V1"] - decimals["010V2"]
                },
                {
                    "5",
                    decimals => decimals["020V1"] + decimals["040V1"] + decimals["050V1"]
                },
                {
                    "6",
                    decimals => decimals["020V2"] + decimals["040V2"] + decimals["050V2"]
                },
                {
                    "7",
                    decimals => decimals["020V1"] + decimals["040V1"] + decimals["050V1"] -
                                (decimals["020V2"] + decimals["040V2"] + decimals["050V2"])
                },
                {
                    "8",
                    decimals => (decimals["020V1"] + decimals["040V1"] + decimals["050V1"]) / decimals["010V1"]
                },
                {
                    "9",
                    decimals => (decimals["020V2"] + decimals["040V2"] + decimals["050V2"]) / decimals["010V2"]
                },
                {
                    "10",
                    decimals => (decimals["020V1"] + decimals["040V1"] + decimals["050V1"]) / decimals["010V1"] -
                                (decimals["020V2"] + decimals["040V2"] + decimals["050V2"]) / decimals["010V2"]
                },
                {
                    "13",
                    decimals => decimals["060V1"] - decimals["060V2"]
                },
                {
                    "16",
                    decimals => decimals["150V1"] - decimals["150V2"]
                },
                {
                    "19",
                    decimals => decimals["210V1"] - decimals["210V2"]
                },
                {
                    "20",
                    decimals => decimals["060V1"] / decimals["010V1"]
                },
                {
                    "21",
                    decimals => decimals["060V2"] / decimals["010V2"]
                },
                {
                    "22",
                    decimals => decimals["150V2"] - decimals["150V1"]
                },
                {
                    "23",
                    decimals => decimals["060V1"] / (decimals["020V1"] + decimals["040V1"] + decimals["050V1"])
                },
                {
                    "24",
                    decimals => decimals["060V2"] / (decimals["020V2"] + decimals["040V2"] + decimals["050V2"])
                },
                {
                    "25",
                    decimals => decimals["060V2"] - decimals["060V1"]
                }
            };

            if (internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                //is industrial
                var firstInd = internalAnalyticsTable
                    .Where(x => x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newInd = GroupBy(calcColumns, firstInd, true, "ИТОГО ПРОМЫШЛЕННЫЕ");
                indSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.Industrial)
                    ? newInd
                    : newInd.GetRange(0, newInd.Count - 1));
            }

            if (internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial))
            {
                //is unIndustrial
                var firstUnInd = internalAnalyticsTable
                    .Where(x => !x.Organization.TypeActivity.IsIndustrial)
                    .OrderBy(x => x.TitleRow).ToList();
                var newUnInd = GroupBy(calcColumns, firstUnInd, false, "ИТОГО НЕПРОМЫШЛЕННЫЕ");
                unIndSort.AddRange(filterModel.Filter.Any(x => x == ReportStatisticsFilterEnum.UnIndustrial)
                    ? newUnInd
                    : newUnInd.GetRange(0, newUnInd.Count - 1));
            }

            var tab1 = new List<AnalyticsModel>();
            tab1.AddRange(indSort);
            tab1.AddRange(unIndSort);
            if (internalAnalyticsTable.Any(x => !x.Organization.TypeActivity.IsIndustrial) &&
                internalAnalyticsTable.Any(x => x.Organization.TypeActivity.IsIndustrial))
            {
                if (tab1.Any(x => x.TitleRow.Contains("ИТОГО")))
                {
                    var (item1, item2) = GetSum(tab1.Where(x => x.TitleRow.Contains("ИТОГО")).ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }

                if (tab1.Any(x =>
                    x.Organization != null &&
                    x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase)))
                {
                    var (item1, item2) = GetSum(
                        tab1.Where(x =>
                                x.Organization != null &&
                                x.Organization.Name.Contains("Концерн", StringComparison.OrdinalIgnoreCase))
                            .ToList(), calcColumns);
                    // total with koncern
                    tab1.Add(new AnalyticsModel
                    {
                        TitleRow = "ИТОГО без концерна",
                        Items = item1,
                        ItemsCalcValues = item2,
                        IsBold = true
                    });
                }
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsHolding))
            {
                var (item1, item2) = GetSum(
                    analyticsTable.Where(x => x.Organization != null && x.Organization.IsHolding)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Итого холдинг Д/О организаций",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей 50% и более",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            if (internalAnalyticsTable.Any(x => x.Organization != null && !x.Organization.IsState))
            {
                var (item1, item2) = GetSum(analyticsTable.Where(x => x.Organization != null && !x.Organization.IsState)
                        .Select(x => (AnalyticsModel) x).ToList(),
                    calcColumns);
                tab1.Add(new AnalyticsModel
                {
                    TitleRow = "Организации с долей менее 50%",
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            return new List<List<Dictionary<string, string>>>
            {
                tab1.Where(x => x.Items != null).Select(x =>
                    x.ToDictionary(maxItemCount: tab1.Max(y => y.Items.Count), isExcelExport: isExport)).ToList(),
            };
        }

        private List<AnalyticsModel> GroupBy(
            Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>> calcColumns, List<AnalyticsModel> models,
            bool isIndustrial,
            string totalString, string[] ignoreIndex = null, string[] ignoreCoefficient = null)
        {
            var newList = new List<AnalyticsModel>();
            var totalList = new List<AnalyticsModel>();
            var orderList = models.OrderBy(x => x.Organization.TypeActivity.Position)
                .ThenBy(x => x.TitleRow).ToList();
            var sortGroup = orderList.GroupBy(x => x.Organization.TypeActivity.Name);
            foreach (var group in sortGroup)
            {
                var groupList = group.ToList();
                newList.AddRange(groupList);
                var (item1, item2) = GetSum(groupList, calcColumns, ignoreCoefficient);

                var totalModel = new AnalyticsModel
                {
                    TitleRow = group.Key,
                    Items = item1,
                    ItemsCalcValues = item2,
                    IsBold = true
                };
                newList.Add(totalModel);
                totalList.Add(totalModel);
            }

            if (totalList.Any())
            {
                var (item1, item2) = GetSum(totalList, calcColumns, ignoreCoefficient);

                newList.Add(new AnalyticsModel
                {
                    TitleRow = totalString,
                    Items = item1.Select(x =>
                        ignoreIndex?.Contains(x.Index) ?? false
                            ? new AnalyticsItemModel {Index = x.Index, Value = null}
                            : x).ToList(),
                    ItemsCalcValues = item2,
                    IsBold = true
                });
            }

            return newList;
        }

        private Tuple<List<AnalyticsItemModel>, Dictionary<string, decimal?>> GetSum(List<AnalyticsModel> list,
            Dictionary<string, Func<Dictionary<string, decimal?>, decimal?>> calcColumns,
            string[] ignoreCoefficient = null)
        {
            if (!list.Any()) return null;
            var values = new decimal?[list[0].Items.Count()];
            var calcValuesSum = new Dictionary<string, decimal?>();
            for (var i = 0; i < list.Count(); i++)
            {
                for (var j = 0; j < list[i].Items.Count(); j++)
                {
                    if (values[j] == null || ignoreCoefficient == null ||
                        !ignoreCoefficient.Contains(list[i].Items[j].Index))
                        values[j] = values[j].GetValueOrDefault() + list[i].Items[j].Value.GetValueOrDefault();
                }

                for (var j = 0; j < list[i].ItemsCalcValues?.Count; j++)
                {
                    if (calcValuesSum.ContainsKey(list[i].ItemsCalcValues.ElementAt(j).Key))
                    {
                        if (ignoreCoefficient == null ||
                            !ignoreCoefficient.Contains(list[i].ItemsCalcValues.ElementAt(j).Key))
                        {
                            calcValuesSum[list[i].ItemsCalcValues.ElementAt(j).Key] =
                                calcValuesSum[list[i].ItemsCalcValues.ElementAt(j).Key].GetValueOrDefault() +
                                list[i].ItemsCalcValues.ElementAt(j).Value.GetValueOrDefault();
                        }
                    }
                    else
                        calcValuesSum.Add(list[i].ItemsCalcValues.ElementAt(j).Key,
                            list[i].ItemsCalcValues.ElementAt(j).Value.GetValueOrDefault());
                }
            }

            var valuesModel = new List<AnalyticsItemModel>();
            for (var i = 0; i < values.Length; i++)
            {
                if (calcColumns.ContainsKey(list[0].Items[i].Index))
                {
                    try
                    {
                        values[i] = calcColumns[list[0].Items[i].Index].Invoke(calcValuesSum);
                    }
                    catch (Exception)
                    {
                        values[i] = null;
                    }
                }

                valuesModel.Add(new AnalyticsItemModel
                {
                    Index = (i + 2).ToString(),
                    Value = values[i]
                });
            }

            return new Tuple<List<AnalyticsItemModel>, Dictionary<string, decimal?>>(valuesModel, calcValuesSum);
            //return valuesModel;
        }

        public async Task<object> GetReportStatisticsByFilter(ReportStatisticsFilterModel filter)
        {
            var statisticsResult = (dynamic) await _reportRepository.GetReportStatisticsByFilter(filter);
            return await GenerateReportsByFilter(filter, statisticsResult);
        }

        public async Task<object> GetReportStatisticsByMultiFilter(ReportStatisticsFilterModel filter)
        {
            var statisticsResult = (dynamic) await _reportRepository.GetReportStatisticsByMultiFilter(filter);
            return await GenerateReportsByFilter(filter, statisticsResult);
        }

        public async Task<ReportModel> GetFullReport(string id = null)
        {
            var fullReport = await _reportRepository.GetReport(id);
            if (fullReport != null)
            {
                var tabs = await _reportTabRepository.GetByCollectionId(fullReport.FormCollectionId);
                var report = new ReportModel
                {
                    Report = fullReport,
                    TabModels = tabs.Select(x => JsonConvert.DeserializeObject<TabModel>(x.Json)).ToList(),
                };
                foreach (var model in report.TabModels)
                {
                    model.Table.Rows = model.Table.Rows.ToList().Select(x =>
                        JsonConvert.DeserializeObject(Regex.Replace(JsonConvert.SerializeObject(x),
                            "\"([A-Z])[\\w\\d]*\":",
                            m => "\"" + m.ToString().Where(char.IsLetter).First().ToString().ToLower() +
                                 m.ToString().Substring(2))));
                }

                return report;
            }

            return null;
        }

        public async Task<Report> GetReportByInterval(string userCheckinIntervalId) =>
            await _reportRepository.GetReportByInterval(userCheckinIntervalId);

        public async Task<IEnumerable<Report>> GetReportsByInterval(string userCheckinIntervalId) =>
            await _reportRepository.GetReportsByInterval(userCheckinIntervalId);

        public async Task<ReportModel> GetFullReportByInterval(string userCheckinIntervalId)
        {
            var fullReport = await GetReportByInterval(userCheckinIntervalId);

            if (fullReport != null)
            {
                var tabs = await _reportTabRepository.GetByCollectionId(fullReport.FormCollectionId);
                var report = new ReportModel
                {
                    Report = fullReport,
                    TabModels = tabs.Select(x => JsonConvert.DeserializeObject<TabModel>(x.Json)).OrderBy(x => x.TabId)
                        .ToList(),
                };
                foreach (var model in report.TabModels)
                {
                    model.Table.Rows = model.Table.Rows.ToList().Select(x =>
                        JsonConvert.DeserializeObject(Regex.Replace(JsonConvert.SerializeObject(x),
                            "\"([A-Z])[\\w\\d]*\":",
                            m => "\"" + m.ToString().Where(char.IsLetter).First().ToString().ToLower() +
                                 m.ToString().Substring(2))));
                    if (model.Attachment is JObject)
                        model.Attachment = MyConverter.ConvertToClass<Attachment>(model.Attachment);
                }

                return report;
            }

            return null;
        }

        public async Task<Report> GetReport(string id) =>
            await _reportRepository.GetReport(id);

        public async Task SaveReport(Report report, string userId)
        {
            report.IsRead = false;
            StatusReport? oldStatus = null;
            AdminStatusReport? oldStatusAdmin = null;
            bool internalReportIsNew = report.IsNew;
            if (!internalReportIsNew)
            {
                var oldReport = await _reportRepository.GetById(report.Id);
                oldStatus = oldReport.StatusReport;
                oldStatusAdmin = oldReport.AdminStatusReport;
            }

            await _reportRepository.Save(report);
            if (internalReportIsNew || report.StatusReport != oldStatus || report.AdminStatusReport != oldStatusAdmin)
            {
                await _logReportRepository.Save(new LogReport()
                {
                    Date = DateTime.Now,
                    ReportId = report.Id,
                    StatusReport = report.StatusReport,
                    AdminStatusReport = report.AdminStatusReport,
                    ApplicationUserId = report.UserId ?? userId
                });
            }
        }


        public async Task<Report> GetReportByPeriod(int year, string periodId, string reportTypeId, string organizationId) =>
           await _reportRepository.GetReportByPeriod(year, periodId, reportTypeId, organizationId);

        public async Task ChangeReportStatus(string reportId, StatusReport userStatus, string userId)
        {
            var report = await _reportRepository.GetReport(reportId);

            var oldAdminStatus = report.AdminStatusReport;

            //отправить первый раз
            if (oldAdminStatus == null && userStatus == StatusReport.Submitted)
            {
                report.StatusReport = StatusReport.Submitted;
                report.AdminStatusReport = AdminStatusReport.New;
                report.Date = DateTime.Now;
            }
            //отправить повторно
            else if (oldAdminStatus == AdminStatusReport.SentForCorrection && userStatus == StatusReport.Submitted)
            {
                report.StatusReport = StatusReport.Submitted;
                report.AdminStatusReport = AdminStatusReport.AfterCorrection;
                report.Date = DateTime.Now;
                report.ReplyDate = null;
            }
            //отправить на корректировку
            else if (userStatus == StatusReport.ForRevision)
            {
                report.StatusReport = StatusReport.ForRevision;
                report.AdminStatusReport = AdminStatusReport.SentForCorrection;
                report.ReplyDate = DateTime.Now;
            }
            // принять отчет
            else if (userStatus == StatusReport.Accepted)
            {
                report.StatusReport = StatusReport.Accepted;
                report.AdminStatusReport = AdminStatusReport.Accepted;
                report.ReplyDate = DateTime.Now;
            }

            await SaveReport(report, userId);
        }

        public async Task ChangeReportStatus(string reportId, StatusReport userStatus, AdminStatusReport adminStatus,
            string userId, DateTime? replyDate, DateTime? returnDate)
        {
            var report = await _reportRepository.GetReport(reportId);

            report.StatusReport = userStatus;
            report.AdminStatusReport = adminStatus;
            report.ReplyDate = replyDate;
            report.ReturnDate = returnDate;

            await SaveReport(report, userId);
        }

        public async Task<byte[]> Export(ReportModel model)
        {
            var folder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources");
            const string excelName = "ExcelForm.xlsx";
            var file = new FileInfo(Path.Combine(folder, excelName));

            using var package = new ExcelPackage(file);
            var workSheets = package.Workbook.Worksheets;

            var tabIndex = 1;
            var letterCode = new[] {4, 3, 3, 3, 3, 3};
            var rowStart = new[] {18, 13, 18, 13, 12, 12};
            var columnStart = new[] {15, 11, 11, 11, 11, 11};
            var columnStart2 = new[] {60, 51, 77, 43, 11, 11};

            var header = model.TabModels.OrderBy(x => x.TabId).FirstOrDefault()?.Header;
            var footer = model.TabModels.OrderBy(x => x.TabId).FirstOrDefault()?.Footer;
            var interval = await _userIntervalService.GetUserIntervalById(model.Report.UserCheckinIntervalId);
            var organization = interval?.Organization;
            foreach (var worksheet in workSheets)
            {
                if (tabIndex > 7)
                    continue;

                var tabModel = model.TabModels.FirstOrDefault(x => x.TabId == tabIndex) ?? new Tabs(
                    int.Parse(interval?.Period?.Name[^1].ToString() ?? "0"),
                    interval.Year).Models.FirstOrDefault(x => x.TabId == tabIndex);
                if (tabModel == null)
                {
                    tabIndex++;
                    continue;
                }

                var footerPosition = new Dictionary<int, Tuple<int, int>[]>
                {
                    {1, new Tuple<int, int>[] {new(95, 1), new(96, 1), new(95, 5), new(96, 5)}},
                    {2, new Tuple<int, int>[] {new(58, 1), new(59, 1), new(58, 5), new(59, 5)}},
                    {3, new Tuple<int, int>[] {new(82, 1), new(83, 1), new(82, 9), new(83, 9)}},
                    {4, new Tuple<int, int>[] {new(61, 1), new(62, 1), new(61, 5), new(62, 5)}},
                    {5, new Tuple<int, int>[] {new(32, 1), new(33, 1), new(32, 6), new(33, 6)}},
                    {6, new Tuple<int, int>[] {new(25, 1), new(26, 1), new(25, 4), new(26, 4)}},
                };

                worksheet.Cells[footerPosition[tabModel.TabId][0].Item1,
                    footerPosition[tabModel.TabId][0].Item2].Value = footer?.LeaderName;
                worksheet.Cells[footerPosition[tabModel.TabId][1].Item1,
                    footerPosition[tabModel.TabId][1].Item2].Value = footer?.AccountantGeneral;
                worksheet.Cells[footerPosition[tabModel.TabId][2].Item1,
                    footerPosition[tabModel.TabId][2].Item2].Value = footer?.Leader;
                worksheet.Cells[footerPosition[tabModel.TabId][3].Item1,
                    footerPosition[tabModel.TabId][3].Item2].Value = footer?.ChiefAccountant;

                if (tabModel.TabId == 1)
                {
                    worksheet.Cells[11, 7].Value = tabModel.AdditionTable?.ApprovedDate != null
                        ? $"{tabModel.AdditionTable.ApprovedDate:dd.MM.yyyy}"
                        : string.Empty;
                    worksheet.Cells[12, 7].Value = tabModel.AdditionTable?.SendDate != null
                        ? $"{tabModel.AdditionTable.SendDate:dd.MM.yyyy}"
                        : string.Empty;
                    worksheet.Cells[13, 7].Value =
                        tabModel.AdditionTable?.AcceptedDate != null
                            ? $"{tabModel.AdditionTable.AcceptedDate:dd.MM.yyyy}"
                            : string.Empty;
                }  
                
                for (var i = 3; i < 10; i++)
                {
                    worksheet.Cells[i, 2].Clear();
                    worksheet.Cells[i, 2].Value = i switch
                    {
                        3 => header?.Organization,
                        4 => header?.Number,
                        5 => header?.TypeEconomicActivity,
                        6 => header?.OrganizationalLegalForm,
                        7 => header?.Government,
                        8 => header?.Unit,
                        9 => header?.Address,
                        _ => worksheet.Cells[3, 2].Value
                    };
                    //{
                    //    3 => header?.Organization,
                    //    4 => header?.Number,
                    //    5 => header?.TypeEconomicActivity,
                    //    6 => header?.OrganizationalLegalForm,
                    //    7 => header?.Government,
                    //    8 => header?.Unit,
                    //    9 => header?.Address,
                    //    _ => worksheet.Cells[3, 2].Value
                    //};
                    worksheet.Cells[i, 2].Style.Border.Bottom.Style =
                        i == 9 ? ExcelBorderStyle.Medium : ExcelBorderStyle.Thin;
                    worksheet.Cells[i, 2].Style.Border.Top.Style =
                        i == 3 ? ExcelBorderStyle.Medium : ExcelBorderStyle.Thin;
                    worksheet.Cells[i, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[i, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                worksheet.Cells[2, 1].Value = $"{tabModel?.Title}\n{tabModel.TitleDate}";

                switch (tabModel.TabId)
                {
                    case 1:
                        worksheet.Cells[3, 2, 3, 8].Merge = true;
                        worksheet.Cells[4, 2, 4, 8].Merge = true;
                        worksheet.Cells[5, 2, 5, 8].Merge = true;
                        worksheet.Cells[6, 2, 6, 8].Merge = true;
                        worksheet.Cells[7, 2, 7, 8].Merge = true;
                        worksheet.Cells[8, 2, 8, 8].Merge = true;
                        worksheet.Cells[9, 2, 9, 8].Merge = true;
                        break;
                    case 2:
                        worksheet.Cells[3, 2, 3, 7].Merge = true;
                        worksheet.Cells[4, 2, 4, 7].Merge = true;
                        worksheet.Cells[5, 2, 5, 7].Merge = true;
                        worksheet.Cells[6, 2, 6, 7].Merge = true;
                        worksheet.Cells[7, 2, 7, 7].Merge = true;
                        worksheet.Cells[8, 2, 8, 7].Merge = true;
                        worksheet.Cells[9, 2, 9, 7].Merge = true;
                        break;
                    case 3:
                        worksheet.Cells[3, 2, 3, 15].Merge = true;
                        worksheet.Cells[4, 2, 4, 15].Merge = true;
                        worksheet.Cells[5, 2, 5, 15].Merge = true;
                        worksheet.Cells[6, 2, 6, 15].Merge = true;
                        worksheet.Cells[7, 2, 7, 15].Merge = true;
                        worksheet.Cells[8, 2, 8, 15].Merge = true;
                        worksheet.Cells[9, 2, 9, 15].Merge = true;
                        break;
                    case 4:
                        worksheet.Cells[3, 2, 3, 7].Merge = true;
                        worksheet.Cells[4, 2, 4, 7].Merge = true;
                        worksheet.Cells[5, 2, 5, 7].Merge = true;
                        worksheet.Cells[6, 2, 6, 7].Merge = true;
                        worksheet.Cells[7, 2, 7, 7].Merge = true;
                        worksheet.Cells[8, 2, 8, 7].Merge = true;
                        worksheet.Cells[9, 2, 9, 7].Merge = true;
                        break;
                    case 5:
                        worksheet.Cells[3, 2, 3, 8].Merge = true;
                        worksheet.Cells[4, 2, 4, 8].Merge = true;
                        worksheet.Cells[5, 2, 5, 8].Merge = true;
                        worksheet.Cells[6, 2, 6, 8].Merge = true;
                        worksheet.Cells[7, 2, 7, 8].Merge = true;
                        worksheet.Cells[8, 2, 8, 8].Merge = true;
                        worksheet.Cells[9, 2, 9, 8].Merge = true;
                        break;
                    case 6:
                        worksheet.Cells[3, 2, 3, 8].Merge = true;
                        worksheet.Cells[4, 2, 4, 8].Merge = true;
                        worksheet.Cells[5, 2, 5, 8].Merge = true;
                        worksheet.Cells[6, 2, 6, 8].Merge = true;
                        worksheet.Cells[7, 2, 7, 8].Merge = true;
                        worksheet.Cells[8, 2, 8, 8].Merge = true;
                        worksheet.Cells[9, 2, 9, 8].Merge = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                worksheet.Cells[3, 2, 9, 2].Style.Font.Bold = true;

                var rowCount = worksheet.Dimension.Rows;

                var columnIndexName = 1;
                for (var i = 0; i < tabModel.Table.Columns.Count(); i++)
                {
                    if (i > 1)
                    {
                        var columnItem = tabModel.Table.Columns[i];
                        worksheet.Cells[columnStart[tabIndex - 1], columnIndexName].Value = columnItem.Title;
                        worksheet.Cells[columnStart2[tabIndex - 1], columnIndexName].Value = columnItem.Title;
                    }

                    GetNextColumn(worksheet, columnStart[tabIndex - 1], ref columnIndexName);
                }

                IEnumerable<object> modelRows;
                switch (tabModel?.TabName)
                {
                    case TabName.BalanceTab:
                        modelRows = new List<BalanceSheetItem>(tabModel?.Table.Rows.All(x => x is JObject) ?? false
                            ? MyConverter.ConvertToClass<BalanceSheetItem>(tabModel?.Table.Rows)
                            : tabModel?.Table.Rows.Select(x => (BalanceSheetItem) x));
                        break;
                    case TabName.ProfitLossTab:
                        modelRows = new List<ProfitLossItem>(tabModel?.Table.Rows.All(x => x is JObject) ?? false
                            ? MyConverter.ConvertToClass<ProfitLossItem>(tabModel?.Table.Rows)
                            : tabModel?.Table.Rows.Select(x => (ProfitLossItem) x));
                        break;
                    case TabName.ChangeEquityTab:
                        modelRows = new List<ChangeEquityItem>(tabModel?.Table.Rows.All(x => x is JObject) ?? false
                            ? MyConverter.ConvertToClass<ChangeEquityItem>(tabModel?.Table.Rows)
                            : tabModel?.Table.Rows.Select(x => (ChangeEquityItem) x));
                        break;
                    case TabName.MoveMoneyTab:
                        modelRows = new List<MoveMoneyItem>(tabModel?.Table.Rows.All(x => x is JObject) ?? false
                            ? MyConverter.ConvertToClass<MoveMoneyItem>(tabModel?.Table.Rows)
                            : tabModel?.Table.Rows.Select(x => (MoveMoneyItem) x));
                        break;
                    case TabName.DecodingAccruedTaxes:
                        modelRows = new List<DecodingAccruedTaxesItem>(
                            tabModel?.Table.Rows.All(x => x is JObject) ?? false
                                ? MyConverter.ConvertToClass<DecodingAccruedTaxesItem>(tabModel?.Table.Rows)
                                : tabModel?.Table.Rows.Select(x => (DecodingAccruedTaxesItem) x));
                        break;
                    case TabName.DecodingFixedAssets:
                        modelRows = new List<DecodingFixedAssetsItem>(
                            tabModel?.Table.Rows.All(x => x is JObject) ?? false
                                ? MyConverter.ConvertToClass<DecodingFixedAssetsItem>(tabModel?.Table.Rows)
                                : tabModel?.Table.Rows.Select(x => (DecodingFixedAssetsItem) x));
                        break;
                    default:
                        modelRows = new List<TableItem>(tabModel?.Table.Rows.All(x => x is JObject) ?? false
                            ? MyConverter.ConvertToClass<TableItem>(tabModel?.Table.Rows)
                            : tabModel?.Table.Rows.Select(x => (TableItem) x));
                        break;
                }

                if (modelRows.Any())
                {
                    for (var i = rowStart[tabIndex - 1]; i <= rowCount; i++)
                    {
                        if (worksheet.Cells[i, letterCode[tabIndex - 1]].Value == null) continue;

                        var valueCell =
                            double.TryParse(worksheet.Cells[i, letterCode[worksheet.Index - 1]].Value.ToString(),
                                out var val)
                                ? val
                                : 0;
                        var row = modelRows?.FirstOrDefault(x =>
                            decimal.Parse(x.GetValue<string>("CodeItem")?.ToString() ?? "1") ==
                            decimal.Parse((valueCell != null && valueCell is double ? valueCell.ToString() : "0") ??
                                          "0"));
                        if (row == null) continue;

                        var column = letterCode[tabIndex - 1];
                        GetNextColumn(worksheet, i, ref column);

                        if (new double[] {10, 40, 131, 110, 140, 050, 060, 100, 150, 160, 200}.Contains(
                                valueCell) && tabModel.TabId == 3
                            || new double[] {120, 130}.Contains(valueCell) &&
                            tabModel.TabId == 4)
                        {
                            ExcelRange mergeCells = null;
                            if (worksheet.Cells[i, 1].Merge)
                            {
                                var mergeLength = 1;
                                var mergeId = worksheet.GetMergeCellId(i, 1);
                                while (worksheet.Cells[i, 1 + mergeLength].Merge &&
                                       worksheet.GetMergeCellId(i, 1 + mergeLength) == mergeId)
                                    mergeLength++;

                                mergeCells = worksheet.Cells[i, 1, i, mergeLength];

                                mergeCells.Merge = false;
                            }

                            worksheet.Cells[i, 1].Clear();
                            worksheet.Cells[i, 1].Value = row.GetType().GetProperty("Title").GetValue(row);

                            if (mergeCells != null)
                            {
                                mergeCells.Style.WrapText = true;
                                mergeCells.Merge = true;
                                mergeCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                mergeCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                mergeCells.Style.Fill.PatternType = ExcelFillStyle.None;
                            }
                            else
                            {
                                worksheet.Cells[i, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[i, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[i, 1].Style.Fill.PatternType = ExcelFillStyle.None;
                            }
                        }

                        foreach (var propertyInfo in row.GetType().GetProperties())
                        {
                            if (!propertyInfo.Name.Contains("Value"))
                                continue;

                            var valueProp = (decimal?) propertyInfo.GetValue(row);
                            worksheet.Cells[i, column].Value = valueProp.GetForExcel(new NumberFormatInfo
                            {
                                NumberDecimalSeparator = ","
                            });
                            worksheet.Cells[i, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            GetNextColumn(worksheet, i, ref column);
                        }
                    }
                }

                tabIndex++;
            }

            if (model.ValidOrganization != null && model.InvalidOrganization != null)
            {
                var worksheet = package.Workbook.Worksheets.Add("Организации в своде");
                worksheet.Cells.Style.Font.Name = "times new roman";
                worksheet.Cells[1, 1, 1, 3].Merge = true;
                worksheet.Cells[1, 1].Value = "Включены:";
                var i = 2;
                if (model.ValidOrganization.Any())
                {
                    foreach (var item in model.ValidOrganization)
                    {
                        worksheet.Cells[i, 2, i, 3].Merge = true;
                        worksheet.Cells[i, 2].Value = item.Name;
                        i++;
                    }

                    i++;
                }

                worksheet.Cells[i, 1, i, 3].Merge = true;
                worksheet.Cells[i, 1].Value = "Не включены:";
                i++;
                if (model.InvalidOrganization.Any())
                {
                    foreach (var item in model.InvalidOrganization)
                    {
                        worksheet.Cells[i, 2, i, 3].Merge = true;
                        worksheet.Cells[i, 2].Value = item.Name;
                        i++;
                    }
                }
            }

            return package.GetAsByteArray();
        }

        public byte[] ExportAnalitycs(AnalyticModel analytics, AnalyticsTypeEnum analyticsTypeEnum, int quarter,
            int year)
        {
            var date = quarter switch
            {
                1 => new DateTime(year, 3, DateTime.DaysInMonth(year, 3)),
                2 => new DateTime(year, 6, DateTime.DaysInMonth(year, 6)),
                3 => new DateTime(year, 9, DateTime.DaysInMonth(year, 9)),
                4 => new DateTime(year, 12, DateTime.DaysInMonth(year, 12)),
                _ => DateTime.Now
            };
            switch (analyticsTypeEnum)
            {
                case AnalyticsTypeEnum.BalanceAsset:
                    return GenerateBalanceAsset(analytics);
                case AnalyticsTypeEnum.BalanceLiabilities:
                    return GenerateBalanceLiabilities(analytics);
                case AnalyticsTypeEnum.FinancialIndicators:
                    return GenerateFinancialIndicators(analytics);
                case AnalyticsTypeEnum.StructureObligations:
                    return GenerateStructureObligations(analytics);
                case AnalyticsTypeEnum.SolvencyRatios:
                    return GenerateSolvencyRatios(analytics);
                case AnalyticsTypeEnum.WorkingCapital:
                    return GenerateWorkingCapital(analytics);
            }

            return null;

            byte[] GenerateBalanceAsset(AnalyticModel analytics)
            {
                var folder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources");
                const string excelName = "BalanceAssetAnalytics.xlsx";
                var file = new FileInfo(Path.Combine(folder, excelName));

                using var package = new ExcelPackage(file);
                var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                var tab1 = analytics.Tab1;
                var tab2 = analytics.Tab2;

                workSheet.Cells[2, 1].Value = $"На {date.ToString("d MMMM yyyy", new CultureInfo("ru"))} г.";

                var rowIndex = 1;
                foreach (var row in tab1)
                {
                    var colIndex = 2;
                    var rows = row.Where(x => x.Key != "isBold");
                    workSheet.Cells[8 + rowIndex, 1].Value = row.FirstOrDefault(x => x.Key == "1").Value;
                    workSheet.Cells[8 + rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    foreach (var col in rows.Where(x => x.Key != "1"))
                    {
                        workSheet.Cells[8 + rowIndex, colIndex].Value =
                            (decimal.TryParse(col.Value ?? "null", out var result) ? (decimal?) result : null)
                            .GetForExcel(new NumberFormatInfo
                            {
                                NumberDecimalSeparator = ","
                            });
                        workSheet.Cells[8 + rowIndex, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        workSheet.Cells[8 + rowIndex, colIndex].Style.HorizontalAlignment =
                            ExcelHorizontalAlignment.Right;
                        colIndex++;
                    }

                    rowIndex++;
                }

                rowIndex = 1;
                workSheet = package.Workbook.Worksheets.Skip(1).FirstOrDefault();
                if (workSheet != null)
                {
                    workSheet.Cells[2, 1].Value = $"На {date.ToString("d MMMM yyyy", new CultureInfo("ru"))} г.";
                    foreach (var row in tab2)
                    {
                        var colIndex = 2;
                        var rows = row.Where(x => x.Key != "isBold");
                        workSheet.Cells[8 + rowIndex, 1].Value = row.FirstOrDefault(x => x.Key == "1").Value;
                        workSheet.Cells[8 + rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        foreach (var col in rows.Where(x => x.Key != "1"))
                        {
                            workSheet.Cells[8 + rowIndex, colIndex].Value =
                                (decimal.TryParse(col.Value ?? "null", out var result) ? (decimal?) result : null)
                                .GetForExcel(new NumberFormatInfo
                                {
                                    NumberDecimalSeparator = ","
                                });
                            workSheet.Cells[8 + rowIndex, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            workSheet.Cells[8 + rowIndex, colIndex].Style.HorizontalAlignment =
                                ExcelHorizontalAlignment.Right;
                            colIndex++;
                        }

                        rowIndex++;
                    }
                }

                return package.GetAsByteArray();
            }

            byte[] GenerateStructureObligations(AnalyticModel analytics)
            {
                var folder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources");
                const string excelName = "StructureObligationsAnalytics.xlsx";
                var file = new FileInfo(Path.Combine(folder, excelName));

                using var package = new ExcelPackage(file);
                var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                workSheet.Cells[2, 1].Value = $"На {date.ToString("d MMMM yyyy", new CultureInfo("ru"))} г.";

                var tab1 = analytics.Tab1;

                var rowIndex = 1;
                foreach (var row in tab1)
                {
                    var colIndex = 2;
                    var rows = row.Where(x => x.Key != "isBold");
                    workSheet.Cells[7 + rowIndex, 1].Value = row.FirstOrDefault(x => x.Key == "1").Value;
                    workSheet.Cells[7 + rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    foreach (var col in rows.Where(x => x.Key != "1"))
                    {
                        workSheet.Cells[7 + rowIndex, colIndex].Value =
                            (decimal.TryParse(col.Value ?? "null", out var result) ? (decimal?) result : null)
                            .GetForExcel(new NumberFormatInfo
                            {
                                NumberDecimalSeparator = ","
                            });
                        workSheet.Cells[7 + rowIndex, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        workSheet.Cells[7 + rowIndex, colIndex].Style.HorizontalAlignment =
                            ExcelHorizontalAlignment.Right;
                        colIndex++;
                    }

                    rowIndex++;
                }

                return package.GetAsByteArray();
            }

            byte[] GenerateBalanceLiabilities(AnalyticModel analytics)
            {
                var folder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources");
                const string excelName = "BalanceLiabilitiesAnalytics.xlsx";
                var file = new FileInfo(Path.Combine(folder, excelName));

                using var package = new ExcelPackage(file);
                var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                workSheet.Cells[2, 1].Value = $"На {date.ToString("d MMMM yyyy", new CultureInfo("ru"))} г.";
                var tab1 = analytics.Tab1;

                var rowIndex = 1;
                foreach (var row in tab1)
                {
                    var colIndex = 2;
                    var rows = row.Where(x => x.Key != "isBold");
                    workSheet.Cells[8 + rowIndex, 1].Value = row.FirstOrDefault(x => x.Key == "1").Value;
                    workSheet.Cells[8 + rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    foreach (var col in rows.Where(x => x.Key != "1"))
                    {
                        workSheet.Cells[8 + rowIndex, colIndex].Value =
                            (decimal.TryParse(col.Value ?? "null", out var result) ? (decimal?) result : null)
                            .GetForExcel(new NumberFormatInfo
                            {
                                NumberDecimalSeparator = ","
                            });
                        workSheet.Cells[8 + rowIndex, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        workSheet.Cells[8 + rowIndex, colIndex].Style.HorizontalAlignment =
                            ExcelHorizontalAlignment.Right;
                        colIndex++;
                    }

                    rowIndex++;
                }

                return package.GetAsByteArray();
            }

            byte[] GenerateFinancialIndicators(AnalyticModel analytics)
            {
                var folder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources");
                const string excelName = "FinancialIndicatorsAnalytics.xlsx";
                var file = new FileInfo(Path.Combine(folder, excelName));

                using var package = new ExcelPackage(file);
                var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                workSheet.Cells[2, 1].Value = $"На {date.ToString("d MMMM yyyy", new CultureInfo("ru"))} г.";
                var tab1 = analytics.Tab1;

                var rowIndex = 1;
                foreach (var row in tab1)
                {
                    var colIndex = 2;
                    var rows = row.Where(x => x.Key != "isBold");
                    workSheet.Cells[7 + rowIndex, 1].Value = row.FirstOrDefault(x => x.Key == "1").Value;
                    workSheet.Cells[7 + rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    foreach (var col in rows.Where(x => x.Key != "1"))
                    {
                        workSheet.Cells[7 + rowIndex, colIndex].Value =
                            (decimal.TryParse(col.Value ?? "null", out var result) ? (decimal?) result : null)
                            .GetForExcel(new NumberFormatInfo
                            {
                                NumberDecimalSeparator = ","
                            });
                        workSheet.Cells[7 + rowIndex, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        workSheet.Cells[7 + rowIndex, colIndex].Style.HorizontalAlignment =
                            ExcelHorizontalAlignment.Right;
                        colIndex++;
                    }

                    rowIndex++;
                }

                return package.GetAsByteArray();
            }

            byte[] GenerateSolvencyRatios(AnalyticModel analytics)
            {
                var folder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources");
                const string excelName = "SolvencyRatiosAnalytics.xlsx";
                var file = new FileInfo(Path.Combine(folder, excelName));

                using var package = new ExcelPackage(file);
                var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                workSheet.Cells[2, 1].Value = $"На {date.ToString("d MMMM yyyy", new CultureInfo("ru"))} г.";
                var tab1 = analytics.Tab1;

                var rowIndex = 1;
                foreach (var row in tab1)
                {
                    var colIndex = 2;
                    var rows = row.Where(x => x.Key != "isBold");
                    workSheet.Cells[6 + rowIndex, 1].Value = row.FirstOrDefault(x => x.Key == "1").Value;
                    workSheet.Cells[6 + rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    foreach (var col in rows.Where(x => x.Key != "1"))
                    {
                        workSheet.Cells[6 + rowIndex, colIndex].Value =
                            (decimal.TryParse(col.Value ?? "null", out var result) ? (decimal?) result : null)
                            .GetForExcel(new NumberFormatInfo
                            {
                                NumberDecimalSeparator = ","
                            });
                        workSheet.Cells[6 + rowIndex, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        workSheet.Cells[6 + rowIndex, colIndex].Style.HorizontalAlignment =
                            ExcelHorizontalAlignment.Right;
                        colIndex++;
                    }

                    rowIndex++;
                }

                return package.GetAsByteArray();
            }

            byte[] GenerateWorkingCapital(AnalyticModel analytics)
            {
                var folder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources");
                const string excelName = "WorkingCapitalAnalytics.xlsx";
                var file = new FileInfo(Path.Combine(folder, excelName));

                using var package = new ExcelPackage(file);
                var workSheet = package.Workbook.Worksheets.FirstOrDefault();
                workSheet.Cells[2, 1].Value = $"На {date.ToString("d MMMM yyyy", new CultureInfo("ru"))} г.";
                var tab1 = analytics.Tab1;

                var rowIndex = 1;
                foreach (var row in tab1)
                {
                    var colIndex = 2;
                    var rows = row.Where(x => x.Key != "isBold");
                    workSheet.Cells[6 + rowIndex, 1].Value = row.FirstOrDefault(x => x.Key == "1").Value;
                    workSheet.Cells[6 + rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    foreach (var col in rows.Where(x => x.Key != "1"))
                    {
                        workSheet.Cells[6 + rowIndex, colIndex].Value =
                            (decimal.TryParse(col.Value ?? "null", out var result) ? (decimal?) result : null)
                            .GetForExcel(new NumberFormatInfo
                            {
                                NumberDecimalSeparator = ","
                            });
                        workSheet.Cells[6 + rowIndex, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        workSheet.Cells[6 + rowIndex, colIndex].Style.HorizontalAlignment =
                            ExcelHorizontalAlignment.Right;
                        colIndex++;
                    }

                    rowIndex++;
                }

                return package.GetAsByteArray();
            }
        }

        public IEnumerable<TabModel> ImportFile(byte[] file, string organization)
        {
            var tabModels = new List<TabModel>();
            using (var ms = new MemoryStream(file))
            {
                using var package = new ExcelPackage(ms);
                var worksheets = package.Workbook.Worksheets;

                var startRow = new[] {18, 13, 18, 13, 12, 12};
                var codeColumn = new[] {4, 3, 3, 3, 3, 3};
                foreach (var worksheet in worksheets)
                {
                    if (worksheet.Index > 6)
                        continue;

                    var tabModel = new Tabs().Models
                        .FirstOrDefault(x => x.TabName == GetTabNameBySheet(worksheet.Name)).DeepClone();
                    if (tabModel != null)
                    {
                        var title = worksheet.Cells[2, 1].Value.ToString().Split("\n");
                        tabModel.TitleDate = title.Length > 1 ? title[1] : null;

                        var rows = tabModel.Table.Rows.ToList();

                        for (var row = startRow[worksheet.Index - 1]; row < worksheet.Dimension.Rows; row++)
                        {
                            var column = codeColumn[worksheet.Index - 1];
                            GetNextColumn(worksheet, row, ref column);
                            if (worksheet.Cells[row, codeColumn[worksheet.Index - 1]].Value == null)
                                continue;

                            var valueCell = double.TryParse(
                                worksheet.Cells[row, codeColumn[worksheet.Index - 1]].Value.ToString(),
                                out var val)
                                ? val
                                : 0;

                            var rowModel = rows.FirstOrDefault(x =>
                                decimal.Parse(x.GetValue<string>("CodeItem")?.ToString() ?? "1") ==
                                decimal.Parse((valueCell != null && valueCell is double ? valueCell.ToString() : "0") ??
                                              "0"));

                            if (rowModel != null)
                            {
                                if (new double[] {10, 40, 131, 110, 140, 050, 060, 100, 150, 160, 200}.Contains(
                                        valueCell) && tabModel.TabId == 3
                                    || new double[] {120, 130}.Contains(valueCell) &&
                                    tabModel.TabId == 4)
                                {
                                    rowModel.GetType().GetProperty("Title").SetValue(rowModel,
                                        worksheet.Cells[row, 1].Value.ToString().Replace("\n", " "));
                                }

                                foreach (var propertyInfo in rowModel.GetType().GetProperties())
                                {
                                    if (!propertyInfo.Name.Contains("Value"))
                                        continue;

                                    decimal tryValue;
                                    var value = decimal.TryParse(
                                        worksheet.Cells[row, column].Value?.ToString() ?? "null",
                                        out tryValue)
                                        ? (decimal?) tryValue
                                        : null;

                                    // var tabBindings = tabModel.Bindings.FirstOrDefault(
                                    //     x => x.Target == ((TableItem) rowModel).CodeItem);
                                    // var bindingsRows = rows.Where(x =>
                                    //     tabBindings?.From.Contains(((TableItem) x)?.CodeItem ?? "NULL") ?? false).ToArray();
                                    // var value = !bindingsRows.Any() ||
                                    //             worksheet.Cells[row, column].Value?.ToString()?.Trim() != "0"
                                    //     ? decimal.TryParse(
                                    //         worksheet.Cells[row, column].Value?.ToString() ?? "null",
                                    //         out tryValue)
                                    //         ? (decimal?) tryValue
                                    //         : null
                                    //     : bindingsRows.Any(x => x.GetValue<decimal?>(propertyInfo.Name) == null)
                                    //         ? null
                                    //         : decimal.TryParse(
                                    //             worksheet.Cells[row, column].Value?.ToString() ?? "null",
                                    //             out tryValue)
                                    //             ? (decimal?) tryValue
                                    //             : null;

                                    propertyInfo.SetValue(rowModel, value);
                                    GetNextColumn(worksheet, row, ref column);
                                }
                            }
                        }

                        // foreach (var row in rows.Where(x =>
                        //     tabModel.Bindings.Any(y =>
                        //         decimal.Parse(y.Target) ==
                        //         decimal.Parse(x.GetValue<string>("CodeItem")?.ToString() ?? "1"))).ToArray())
                        // {
                        if (tabModel.TabId != 3 && tabModel.TabId != 6)
                            foreach (var row in rows.Where(x =>
                                tabModel.Bindings?.Any(y =>
                                    decimal.TryParse(y.Target, out var targetValue) &&
                                    decimal.TryParse(x.GetValue<string>("CodeItem")?.ToString() ?? "1",
                                        out var rowCode) && targetValue == rowCode) ?? false).ToArray())
                            {
                                var bindingsRows = rows.Where(x =>
                                    tabModel.Bindings.Any(y =>
                                        decimal.TryParse(y.Target, out var targetValue) &&
                                        decimal.TryParse(row.GetValue<string>("CodeItem")?.ToString() ?? "1",
                                            out var mainRowCode) && targetValue == mainRowCode &&
                                        decimal.TryParse(x.GetValue<string>("CodeItem")?.ToString() ?? "1",
                                            out var rowCode)
                                        && (y?.From?.Split(',')
                                            .Select(decimal.Parse).Contains(
                                                rowCode) ?? false))).ToArray();

                                foreach (var propertyInfo in row.GetType().GetProperties())
                                {
                                    if (!propertyInfo.Name.Contains("Value"))
                                        continue;

                                    decimal tryValue;

                                    // var tabBindings = tabModel.Bindings.FirstOrDefault(
                                    //     x => x.Target == ((TableItem) rowModel).CodeItem);
                                    // var bindingsRows = rows.Where(x =>
                                    //     tabBindings?.From.Contains(((TableItem) x)?.CodeItem ?? "NULL") ?? false).ToArray();
                                    // var value = !bindingsRows.Any() ||
                                    //             row.GetValue<decimal?>(propertyInfo.Name) != decimal.Parse("0")
                                    //     ? decimal.TryParse(
                                    //         worksheet.Cells[row, column].Value?.ToString() ?? "null",
                                    //         out tryValue)
                                    //         ? (decimal?) tryValue
                                    //         : null
                                    //     : bindingsRows.Any(x => x.GetValue<decimal?>(propertyInfo.Name) == null)
                                    //         ? null
                                    //         : decimal.TryParse(
                                    //             worksheet.Cells[row, column].Value?.ToString() ?? "null",
                                    //             out tryValue)
                                    //             ? (decimal?) tryValue
                                    //             : null;

                                    // if ((bindingsRows.Any() ||
                                    //      row.GetValue<decimal?>(propertyInfo.Name) == decimal.Parse("0"))
                                    //     && bindingsRows.All(
                                    //         x => x.GetValue<decimal?>(propertyInfo.Name) == null))
                                    if (bindingsRows.All(
                                        x => x.GetValue<decimal?>(propertyInfo.Name) == null))
                                        propertyInfo.SetValue(row, null);

                                    //propertyInfo.SetValue(row, value);
                                }
                            }

                        #region Columns

                        if (tabModel.TabId == 1)
                        {
                            tabModel.Table.Columns[2].Title = new Regex("\\.\\ +").Replace(
                                new Regex("([\\s]+)").Replace(
                                    worksheet.Cells[15, 5].Value.ToString().Trim(),
                                    " "), ".");
                            tabModel.Table.Columns[3].Title = new Regex("\\.\\ +").Replace(
                                new Regex("([\\s]+)").Replace(
                                    worksheet.Cells[15, 7].Value.ToString().Trim(),
                                    " "), ".");
                        }

                        if (tabModel.TabId == 4)
                        {
                            tabModel.Table.Columns[2].Title = new Regex("\\.\\ +").Replace(
                                new Regex("([\\s]+)").Replace(
                                    worksheet.Cells[11, 5].Value.ToString().Trim(),
                                    " "), ".");
                            tabModel.Table.Columns[3].Title = new Regex("\\.\\ +").Replace(
                                new Regex("([\\s]+)").Replace(
                                    worksheet.Cells[11, 6].Value.ToString().Trim(),
                                    " "), ".");
                        }

                        #endregion

                        tabModel.Header.Organization = organization;

                        #region AdditionTable

                        var additionCoordinate = ExcelData.Values.FirstOrDefault(x =>
                            x.Key == "AcceptedDate" && x.TabName == tabModel.TabName);
                        if (additionCoordinate != null)
                        {
                            tabModel.AdditionTable = new AdditionTable();
                            var additionDate = worksheet.Cells[additionCoordinate!.Row, additionCoordinate!.Column]
                                .Value;
                            tabModel.AdditionTable.AcceptedDate =
                                additionDate != null && DateTime.TryParse(additionDate.ToString()!, out var date1)
                                    ? date1
                                    : (DateTime?) null;

                            additionCoordinate = ExcelData.Values.FirstOrDefault(x =>
                                x.Key == "SendDate" && x.TabName == tabModel.TabName);
                            additionDate = worksheet.Cells[additionCoordinate!.Row, additionCoordinate!.Column]
                                .Value;
                            tabModel.AdditionTable.AcceptedDate =
                                additionDate != null && DateTime.TryParse(additionDate.ToString()!, out date1)
                                    ? date1
                                    : (DateTime?) null;

                            additionCoordinate = ExcelData.Values.FirstOrDefault(x =>
                                x.Key == "ApprovedDate" && x.TabName == tabModel.TabName);
                            additionDate = worksheet.Cells[additionCoordinate!.Row, additionCoordinate!.Column]
                                .Value;
                            tabModel.AdditionTable.AcceptedDate =
                                additionDate != null && DateTime.TryParse(additionDate.ToString()!, out date1)
                                    ? date1
                                    : (DateTime?) null;
                        }

                        #endregion

                        #region Footer

                        var coordinate =
                            ExcelData.Values.FirstOrDefault(x => x.Key == "Date" && x.TabName == tabModel.TabName);
                        var excelDate = worksheet.Cells[coordinate!.Row, coordinate!.Column]
                            .Value;
                        tabModel.Footer.Date =
                            excelDate != null && DateTime.TryParse(excelDate.ToString()!, out var date)
                                ? date
                                : (DateTime?) null;
                        coordinate =
                            ExcelData.Values.FirstOrDefault(x => x.Key == "Leader" && x.TabName == tabModel.TabName);
                        tabModel.Footer.Leader =
                            worksheet.Cells[coordinate!.Row, coordinate!.Column].Value?.ToString();
                        tabModel.Footer.LeaderName =
                            worksheet.Cells[coordinate!.Row, 1].Value?.ToString();

                        coordinate =
                            ExcelData.Values.FirstOrDefault(x =>
                                x.Key == "ChiefAccountant" && x.TabName == tabModel.TabName);
                        tabModel.Footer.ChiefAccountant =
                            worksheet.Cells[coordinate!.Row, coordinate!.Column].Value?.ToString();
                        tabModel.Footer.AccountantGeneral =
                            worksheet.Cells[coordinate!.Row, 1].Value?.ToString();

                        #endregion

                        tabModels.Add(tabModel);
                    }
                }
            }

            return tabModels;
        }

        public IEnumerable<TabModel> ImportFileByPeriod(byte[] file, string organization, int? quarter, int? year)
        {
            var tabModels = new List<TabModel>();
            using (var ms = new MemoryStream(file))
            {
                using var package = new ExcelPackage(ms);
                var worksheets = package.Workbook.Worksheets;

                var startRow = new[] {18, 13, 18, 13, 12, 12};
                var codeColumn = new[] {4, 3, 3, 3, 3, 3};
                var WorksheetHeaderData = worksheets.FirstOrDefault();
                foreach (var worksheet in worksheets)
                {
                    if (worksheet.Index > 6)
                        continue;

                    var tabModel = new Tabs(quarter, year).Models
                        .FirstOrDefault(x => x.TabName == GetTabNameBySheet(worksheet.Name)).DeepClone();
                    if (tabModel != null)
                    {
                        var rows = tabModel.Table.Rows.ToList();

                        for (var row = startRow[worksheet.Index - 1]; row < worksheet.Dimension.Rows; row++)
                        {
                            var column = codeColumn[worksheet.Index - 1];
                            GetNextColumn(worksheet, row, ref column);
                            if (worksheet.Cells[row, codeColumn[worksheet.Index - 1]].Value == null)
                                continue;

                            var valueCell = double.TryParse(
                                worksheet.Cells[row, codeColumn[worksheet.Index - 1]].Value.ToString(),
                                out var val)
                                ? val
                                : 0;

                            var rowModel = rows.FirstOrDefault(x =>
                                decimal.Parse(x.GetValue<string>("CodeItem")?.ToString() ?? "1") ==
                                decimal.Parse((valueCell != null && valueCell is double ? valueCell.ToString() : "0") ??
                                              "0"));

                            if (rowModel != null)
                            {
                                foreach (var propertyInfo in rowModel.GetType().GetProperties())
                                {
                                    if (!propertyInfo.Name.Contains("Value"))
                                        continue;

                                    decimal tryValue;
                                    var value = decimal.TryParse(
                                        worksheet.Cells[row, column].Value?.ToString() ?? "null",
                                        out tryValue)
                                        ? (decimal?) tryValue
                                        : null;
                                    propertyInfo.SetValue(rowModel, value);
                                    GetNextColumn(worksheet, row, ref column);
                                }
                            }
                        }

                        if (tabModel.TabId != 3 && tabModel.TabId != 6)
                            foreach (var row in rows.Where(x =>
                                tabModel.Bindings?.Any(y =>
                                    decimal.TryParse(y.Target, out var targetValue) &&
                                    decimal.TryParse(x.GetValue<string>("CodeItem")?.ToString() ?? "1",
                                        out var rowCode) && targetValue == rowCode) ?? false).ToArray())
                            {
                                var bindingsRows = rows.Where(x =>
                                    tabModel.Bindings.Any(y =>
                                        decimal.TryParse(y.Target, out var targetValue) &&
                                        decimal.TryParse(row.GetValue<string>("CodeItem")?.ToString() ?? "1",
                                            out var mainRowCode) && targetValue == mainRowCode &&
                                        decimal.TryParse(x.GetValue<string>("CodeItem")?.ToString() ?? "1",
                                            out var rowCode)
                                        && (y?.From?.Split(',')
                                            .Select(decimal.Parse).Contains(
                                                rowCode) ?? false))).ToArray();

                                foreach (var propertyInfo in row.GetType().GetProperties())
                                {
                                    if (!propertyInfo.Name.Contains("Value"))
                                        continue;

                                    decimal tryValue;

                                    if (bindingsRows.All(
                                        x => x.GetValue<decimal?>(propertyInfo.Name) == null))
                                        propertyInfo.SetValue(row, null);
                                    ;
                                }
                            }

                        tabModel.Header.Organization = organization;
                        tabModel.Header.Number = string.IsNullOrWhiteSpace(WorksheetHeaderData.Cells[4, 2].Text)
                            ? tabModel.Header.Number
                            : WorksheetHeaderData.Cells[4, 2].Text ?? tabModel.Header.Number;

                        tabModel.Header.TypeEconomicActivity =
                            string.IsNullOrWhiteSpace(WorksheetHeaderData.Cells[5, 2].Text)
                                ? tabModel.Header.TypeEconomicActivity
                                : WorksheetHeaderData.Cells[5, 2].Text ?? tabModel.Header.TypeEconomicActivity;

                        tabModel.Header.OrganizationalLegalForm =
                            string.IsNullOrWhiteSpace(WorksheetHeaderData.Cells[6, 2].Text)
                                ? tabModel.Header.OrganizationalLegalForm
                                : WorksheetHeaderData.Cells[6, 2].Text ?? tabModel.Header.OrganizationalLegalForm;

                        tabModel.Header.Government = string.IsNullOrWhiteSpace(WorksheetHeaderData.Cells[7, 2].Text)
                            ? tabModel.Header.Government
                            : WorksheetHeaderData.Cells[7, 2].Text ?? tabModel.Header.Government;

                        tabModel.Header.Unit = string.IsNullOrWhiteSpace(WorksheetHeaderData.Cells[8, 2].Text)
                            ? tabModel.Header.Unit
                            : WorksheetHeaderData.Cells[8, 2].Text ?? tabModel.Header.Unit;

                        tabModel.Header.Address = string.IsNullOrWhiteSpace(WorksheetHeaderData.Cells[9, 2].Text)
                            ? tabModel.Header.Address
                            : WorksheetHeaderData.Cells[9, 2].Text ?? tabModel.Header.Address;

                        #region AdditionTable

                        var additionCoordinate = ExcelData.Values.FirstOrDefault(x =>
                            x.Key == "AcceptedDate" && x.TabName == tabModel.TabName);
                        if (additionCoordinate != null)
                        {
                            tabModel.AdditionTable = new AdditionTable();
                            var additionDate = worksheet.Cells[additionCoordinate!.Row, additionCoordinate!.Column]
                                .Value;
                            tabModel.AdditionTable.AcceptedDate =
                                additionDate != null && DateTime.TryParse(additionDate.ToString()!, out var date1)
                                    ? date1
                                    : (DateTime?) null;

                            additionCoordinate = ExcelData.Values.FirstOrDefault(x =>
                                x.Key == "SendDate" && x.TabName == tabModel.TabName);
                            additionDate = worksheet.Cells[additionCoordinate!.Row, additionCoordinate!.Column]
                                .Value;
                            tabModel.AdditionTable.AcceptedDate =
                                additionDate != null && DateTime.TryParse(additionDate.ToString()!, out date1)
                                    ? date1
                                    : (DateTime?) null;

                            additionCoordinate = ExcelData.Values.FirstOrDefault(x =>
                                x.Key == "ApprovedDate" && x.TabName == tabModel.TabName);
                            additionDate = worksheet.Cells[additionCoordinate!.Row, additionCoordinate!.Column]
                                .Value;
                            tabModel.AdditionTable.AcceptedDate =
                                additionDate != null && DateTime.TryParse(additionDate.ToString()!, out date1)
                                    ? date1
                                    : (DateTime?) null;
                        }

                        #endregion

                        #region Footer

                        var coordinate =
                            ExcelData.Values.FirstOrDefault(x => x.Key == "Date" && x.TabName == tabModel.TabName);
                        var excelDate = worksheet.Cells[coordinate!.Row, coordinate!.Column]
                            .Value;
                        tabModel.Footer.Date =
                            excelDate != null && DateTime.TryParse(excelDate.ToString()!, out var date)
                                ? date
                                : (DateTime?) null;
                        coordinate =
                            ExcelData.Values.FirstOrDefault(x => x.Key == "Leader" && x.TabName == tabModel.TabName);
                        tabModel.Footer.Leader =
                            worksheet.Cells[coordinate!.Row, coordinate!.Column].Value?.ToString();
                        tabModel.Footer.LeaderName =
                            worksheet.Cells[coordinate!.Row, 1].Value?.ToString();

                        coordinate =
                            ExcelData.Values.FirstOrDefault(x =>
                                x.Key == "ChiefAccountant" && x.TabName == tabModel.TabName);
                        tabModel.Footer.ChiefAccountant =
                            worksheet.Cells[coordinate!.Row, coordinate!.Column].Value?.ToString();
                        tabModel.Footer.AccountantGeneral =
                            worksheet.Cells[coordinate!.Row, 1].Value?.ToString();

                        #endregion

                        tabModels.Add(tabModel);
                    }
                }
            }

            return tabModels;
        }

        private TabName GetTabNameBySheet(string sheetName)
        {
            return sheetName switch
            {
                "1" => TabName.BalanceTab,
                "2" => TabName.ProfitLossTab,
                "3" => TabName.ChangeEquityTab,
                "4" => TabName.MoveMoneyTab,
                "6" => TabName.DecodingAccruedTaxes,
                "7" => TabName.DecodingFixedAssets,
                _ => TabName.BalanceTab
            };
        }

        private void GetNextColumn(ExcelWorksheet worksheet, int row, ref int current)
        {
            var mergeId = worksheet.GetMergeCellId(row, current);
            if (mergeId != 0)
            {
                var columns = worksheet.MergedCells[mergeId - 1];
                var arr = columns.Split(":");
                var diffValue = worksheet.Cells[arr[1]].Start.Column -
                                worksheet.Cells[arr[0]].Start.Column;
                current += diffValue + 1;
            }
            else
            {
                current++;
            }
        }

        private async Task<object> GenerateReportsByFilter(ReportStatisticsFilterModel filter, dynamic statisticsResult)
        {
            var reportsWithFullData = new ReportModel[statisticsResult.validReports.Length];
            var i = 0;
            foreach (var report in statisticsResult.validReports)
            {
                reportsWithFullData[i] = await GetFullReport(report.Id);
                i++;
            }

            var reportWithMaxTab = reportsWithFullData.Length == 0
                ? new {value = new ReportModel(), tabCount = 0}
                : reportsWithFullData
                    .Select(value => new {value, tabCount = value.TabModels.Count})
                    .Aggregate((x, y) => x.tabCount >= y.tabCount ? x : y);
            var organizationName = GetReportOrganizationNameByFilter(filter.Filter.FirstOrDefault());
            var resultReport = new ReportModel
            {
                Report = reportWithMaxTab.value.Report,
                TabModels =
                    new Tabs(Convert.ToInt32((await _periodRepository.GetById(filter.PeriodId)).Name.Last().ToString()),
                        filter.Year).Models.Select(x => x.DeepClone()).Select(x => new TabModel
                    {
                        Attachment = MyConverter.ConvertToClass<Attachment>(x.Attachment),
                        Header = new Header
                        {
                            Organization = organizationName,
                            Government = "Совет Министров",
                            Unit = "тыс. руб"
                        },
                        Title = x.Title,
                        TitleDate = x.TitleDate,
                        Footer = new Footer(),
                        AdditionTable = x.AdditionTable,
                        Table = new Table
                        {
                            Columns = x.Table.Columns.ToList(),
                            Rows = reportWithMaxTab.value.TabModels?.Any(t => t.TabName == x.TabName) ?? false
                                ? reportWithMaxTab.value.TabModels.FirstOrDefault(t => t.TabName == x.TabName)?.Table
                                      .Rows
                                  ?? x.Table.Rows
                                : x.Table.Rows.ToList()
                        },
                        TabId = x.TabId,
                        TabName = x.TabName,
                        ReportTypeName = x.ReportTypeName,
                        Bindings = x.Bindings,
                        Validations = x.Validations,
                        SubtractedColumns = x.SubtractedColumns,
                        SubtractedRows = x.SubtractedRows,
                        ReadOnlyCells = x.ReadOnlyCells
                    }).ToList()
            };
            var reportWithMaxTabEmpty = reportWithMaxTab.value.TabModels?.Count == null &&
                                        (!reportWithMaxTab.value.TabModels?.Any() ?? true);
            if (!reportWithMaxTabEmpty)
                foreach (var tabModel in resultReport.TabModels)
                {
                    var tabModels =
                        reportsWithFullData.Select(x => x.TabModels.Where(x => x.TabId == tabModel.TabId).ToArray());

                    var t = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x =>
                        x.IsClass &&
                        x.GetCustomAttribute(typeof(TabItemEnumReferenceAttribute), false) is
                            TabItemEnumReferenceAttribute
                            attribute && attribute.TabNameProperty == tabModel.TabName &&
                        x.Namespace.Contains("models", StringComparison.OrdinalIgnoreCase) &&
                        x.IsSubclassOf(typeof(TableItem)));
                    tabModel.Table.Rows = tabModel.Table.Rows.Select(x => MyConverter.ConvertToClass(x, t)).ToArray();
                    var propertyInfos = t
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(x => x.Name.Contains("value", StringComparison.OrdinalIgnoreCase));
                    foreach (var property in propertyInfos)
                    {
                        i = 0;
                        foreach (var row in tabModel.Table.Rows)
                        {
                            var tabValues = new decimal?[tabModels.Count()];
                            var point = 0;
                            foreach (var x in tabModels)
                            {
                                var values = new decimal?[x.Count(x => x != null)];
                                var point1 = 0;
                                foreach (var y in x.Where(x => x != null))
                                {
                                    values[point1] = GetTabModelValue(y.Table.Rows, property, t, i);
                                    point1++;
                                }

                                tabValues[point] = values.Any(z => z.HasValue) ? values.Sum() : null;
                                point++;
                            }

                            var sum = tabValues.Any(z => z.HasValue) ? tabValues.Sum() : null;
                            property.SetValue(row, sum);
                            i++;
                        }
                    }
                }

            return reportWithMaxTabEmpty
                ? new
                {
                    report = (object) null,
                    tabModels = (object) null,
                    validOrganization = (object) null,
                    invalidOrganization = (object) null
                }
                : new
                {
                    resultReport.Report,
                    resultReport.TabModels,
                    statisticsResult.validOrganization,
                    statisticsResult.invalidOrganization
                };

            string GetReportOrganizationNameByFilter(ReportStatisticsFilterEnum filterEnum)
            {
                return filterEnum switch
                {
                    ReportStatisticsFilterEnum.PercentageOfStateOwnership => "Свод по Концерну с Аппаратом",
                    ReportStatisticsFilterEnum.Full => "Свод по Концерну с Аппаратом",
                    ReportStatisticsFilterEnum.WithoutOrganizationGroup => "Свод по Концерну без Аппарата",
                    ReportStatisticsFilterEnum.Industrial => "Свод по Промышленным предприятиям",
                    ReportStatisticsFilterEnum.Woodworking => "Свод по Деревообрабатывающим предприятиям",
                    ReportStatisticsFilterEnum.PPIEnterprises => "Свод по Предприятиям ЦБП",
                    ReportStatisticsFilterEnum.Furniture => "Свод по Мебельным предприятиям",
                    ReportStatisticsFilterEnum.UnIndustrial => "Свод по Непромышленным предприятиям",
                    ReportStatisticsFilterEnum.LoggingOrganizations => "Свод по Лесозаготовительным организациям",
                    ReportStatisticsFilterEnum.TradeEnterprises => "Свод по Торговым организациям",
                    ReportStatisticsFilterEnum.Others => "Свод по Прочим организациям",
                    ReportStatisticsFilterEnum.PercentageMoreThenHalf => "Свод по организациям с долей 50% и более",
                    ReportStatisticsFilterEnum.PercentageLessThenHalf => "Свод по организациям с долей до 50%",
                    ReportStatisticsFilterEnum.IsDOHolding => "Свод по Холдингу ДО организаций",
                    _ => "Свод по Концерну с Аппаратом"
                };
            }
        }

        private decimal? GetTabModelValue(IEnumerable<object> rows, PropertyInfo propertyInfo, Type t, int rowIndex)
        {
            object? row = rows.ToArray()[rowIndex];
            if (row == null)
                return null;
            return propertyInfo == null ? null : propertyInfo.GetValue(MyConverter.ConvertToClass(row, t)) as decimal?;
        }

        private static T _cast<T>(object obj) => (T) obj;
    }
}