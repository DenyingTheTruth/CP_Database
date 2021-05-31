using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Context;
using forest_report_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class TypeActivityRepository : ITypeActivityRepository
    {
        public async Task<List<TypeActivity>> GetAll()
        {
            var context = new ApplicationDbContext();
            return await context.TypeActivities.OrderBy(x => x.Position).ToListAsync();
        }

        public async Task<TypeActivity> GetById(string id)
        {
            var context = new ApplicationDbContext();
            return await context.TypeActivities.FindAsync(id);
        }

        public async Task<int> Save(TypeActivity model)
        {
            var context = new ApplicationDbContext();

            if (model.IsNew)
            {
                model.Id = Guid.NewGuid().ToString();
                await context.TypeActivities.AddAsync(model);
            }
            else
                context.TypeActivities.Update(model);

            return await context.SaveChangesAsync();
        }

        public async Task<int> Remove(string id)
        {
            var context = new ApplicationDbContext();
            var item = await GetById(id);
            context.TypeActivities.Remove(item);
            return await context.SaveChangesAsync();
        }
    }
}