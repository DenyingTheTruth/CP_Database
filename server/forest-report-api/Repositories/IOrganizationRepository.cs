using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Entities;

namespace forest_report_api.Repositories
{
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
        Task<Organization> GetByUserId(string userId);
        Task<List<Organization>> GetFree();
        Task<int> SaveGeneralInfo(Organization model);
    }
}