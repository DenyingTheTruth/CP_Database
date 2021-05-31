using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Context;
using forest_report_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class ReportTabRepository : IReportTabRepository
    {
        public Task<List<BaseFormRep>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<BaseFormRep> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Save(BaseFormRep model)
        {
            var context = new ApplicationDbContext();

            if (model.IsNew)
            {
                model.Id = Guid.NewGuid().ToString();
                await context.Tabs.AddAsync(model);
            }
            else
            {
                context.Tabs.Update(model);
            }

            return await context.SaveChangesAsync();
        }

        public Task<int> Remove(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BaseFormRep>> GetByCollectionId(string collectionId)
        {
            var context = new ApplicationDbContext();
            return await context.Tabs.Where(x => x.CollectionId == collectionId).ToListAsync();
        }
    }
}