using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Models;

namespace forest_report_api.Repositories
{
    public interface IUserCheckinIntervalRepository : IBaseRepository<UserCheckinInterval>
    {
         Task<List<UserCheckinInterval>> GetByOrganization(string organizationId, int? year = null);
         Task<List<UserCheckinInterval>> GetByYearAndReportType(int year, string reportTypeId);
        Task<List<UserCheckinInterval>> GetFree(string organizationId, string reportTypeId, int year);
    }
}