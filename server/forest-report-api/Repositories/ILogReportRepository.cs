using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Entities;

namespace forest_report_api.Repositories
{
    public interface ILogReportRepository : IBaseRepository<LogReport>
    {
        Task<List<LogReport>> GetAll(string userId = null);
    }
}