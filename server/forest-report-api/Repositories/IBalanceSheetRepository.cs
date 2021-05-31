using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Entities;

namespace forest_report_api.Repositories
{
    public interface IReportTabRepository : IBaseRepository<BaseFormRep>
    {
        Task<List<BaseFormRep>> GetByCollectionId(string collectionId);
    }
}