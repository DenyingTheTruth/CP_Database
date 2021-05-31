using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Context;
using forest_report_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class PeriodRepository : IPeriodRepository
    {
        public async Task<List<Period>> GetAll()
        {
            var context = new ApplicationDbContext();

            return await context.Periods
                .Include(period => period.ReportTypes)
                .ThenInclude(periodReport => periodReport.ReportType).ToListAsync();
        }

        public async Task<Period> GetById(string id)
        {
            var context = new ApplicationDbContext();
            return await context.Periods.Where(x => x.Id == id)
                .Include(order => order.ReportTypes)
                .ThenInclude(orderProducts => orderProducts.ReportType).FirstOrDefaultAsync();
        }

        public async Task<int> Save(Period model)
        {
            var context = new ApplicationDbContext();

            if (model.IsNew)
            {
                model.Id = Guid.NewGuid().ToString();
                await context.Periods.AddAsync(model);
            }
            else
                context.Periods.Update(model);

            return await context.SaveChangesAsync();
        }

        public async Task<int> Remove(string id)
        {
            var context = new ApplicationDbContext();
            var item = await GetById(id);
            context.Periods.Remove(item);
            return await context.SaveChangesAsync();
        }
    }
}