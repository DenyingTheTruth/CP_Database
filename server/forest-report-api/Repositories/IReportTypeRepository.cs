using System.Threading.Tasks;
using forest_report_api.Entities;

namespace forest_report_api.Repositories
{
    public interface IReportTypeRepository : IBaseRepository<ReportType>
    {
        Task<ReportType> GetByName(string name);
    }
}