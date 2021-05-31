using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;
using forest_report_api.Models;

namespace forest_report_api.Repositories
{
    public interface IReportRepository : IBaseRepository<Report>
    {
        Task<Report> GetReport(string id);
        Task<Report> GetReportByInterval(string userCheckinIntervalId);
        Task<IEnumerable<Report>> GetReportsByInterval(string userCheckinIntervalId);
        Task<List<Report>> GetSentReports();
        Task<List<Report>> GetAll(string userId = null);
        Task<List<Report>> GetAllByStatus(StatusReport userStatusReport, string userId = null);
        Task<object> GetReportStatistics(ReportStatisticsFilterModel filter);
        Task<object> GetReportStatisticsByFilter(ReportStatisticsFilterModel filter);
        Task<object> GetReportStatisticsByMultiFilter(ReportStatisticsFilterModel filter);
        Task<string[]> GetReportIdByMultiFilter(ReportStatisticsFilterModel filter);
        Task<int> GetReportCount(Expression<Func<Report, bool>> predicate);
        Task SetReportRead(string id, bool read);
        Task<Report> GetReportByPeriod(int year, string periodId, string reportTypeId, string organizationId);
    }
}