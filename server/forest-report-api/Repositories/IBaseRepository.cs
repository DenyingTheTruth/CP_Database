using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Entities;

namespace forest_report_api.Repositories
{
    public interface IBaseRepository<T>
    {
        Task<List<T>> GetAll();
        Task<T> GetById(string id);
        Task<int> Save(T model);
        Task<int> Remove(string id);
    }
}