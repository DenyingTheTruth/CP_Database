using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;
using forest_report_api.Facade;
using forest_report_api.Models;

namespace forest_report_api.Service
{
    public interface IReportService
    {
        Task<List<LogReport>> GetLogReports(string userId = null);
        Task<LogReport> GetLogReport(string id);
        Task<List<Report>> GetReports(string userId = null);
        Task<object> GetReportStatisticsByFilter(ReportStatisticsFilterModel filter);
        Task<object> GetReportStatisticsByMultiFilter(ReportStatisticsFilterModel filter);
        Task<List<Report>> GetSentReports();
        Task<ReportModel> GetFullReport(string id);
        Task<Report> GetReportByInterval(string userCheckinIntervalId);
        Task<IEnumerable<Report>> GetReportsByInterval(string userCheckinIntervalId);
        Task<ReportModel> GetFullReportByInterval(string userCheckinIntervalId);
        Task SaveReport(Report report, string userId);
        Task<Report> GetReport(string reportId);
        Task ChangeReportStatus(string reportId, StatusReport userStatus, string userId);
        Task ChangeReportStatus(string reportId, StatusReport userStatus, AdminStatusReport adminStatus, string userId,
            DateTime? replyDate, DateTime? returnDate);
        Task<byte[]> Export(ReportModel model);
        byte[] ExportAnalitycs(AnalyticModel analytics,AnalyticsTypeEnum analyticsTypeEnum, int quarter, int year);
        IEnumerable<TabModel>ImportFile(byte[] file, string organization);
        IEnumerable<TabModel> ImportFileByPeriod(byte[] file, string organization, int? quarter, int? year);
        Task<List<Report>> GetReportsRevision(string userId = null);
        Task<List<List<Dictionary<string, string>>>> GetBalanceAsset(ReportStatisticsFilterModel filterModel, bool isExport = false);
        Task<List<List<Dictionary<string, string>>>> GetStructureObligations(ReportStatisticsFilterModel filterModel, bool isExport = false);
        Task<List<List<Dictionary<string, string>>>> GetBalanceSheetLiabilitiesStructure(ReportStatisticsFilterModel filterModel, bool isExport = false);
        Task<List<List<Dictionary<string, string>>>> GetFinancialIndicators(ReportStatisticsFilterModel filterModel, bool isExport = false);
        Task<List<List<Dictionary<string, string>>>> GetSolvencyRatios(ReportStatisticsFilterModel filterModel, bool isExport = false);
        Task<List<List<Dictionary<string, string>>>> GetStatusOfOwnWorkingCapital(ReportStatisticsFilterModel filterModel, bool isExport = false);
        Task<int> GetCountUnreadReports();
        Task SetReportRead(string id, bool read);
        Task<Report> GetReportByPeriod(int year, string periodId, string reportTypeId, string organizationId);
    }
}