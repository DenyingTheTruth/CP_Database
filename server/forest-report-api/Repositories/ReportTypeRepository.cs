using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Context;
using forest_report_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class ReportTypeRepository : IReportTypeRepository
    {
        public async Task<List<ReportType>> GetAll()
        {
            var context = new ApplicationDbContext();
            return await context.ReportTypes
                .Include(period => period.Periods)
                .ThenInclude(periodReport => periodReport.Period).ToListAsync();
        }

        public async Task<ReportType> GetById(string id)
        {
            var context = new ApplicationDbContext();
            return await context.ReportTypes.Where(x => x.Id == id)
                .Include(period => period.Periods)
                .ThenInclude(periodReport => periodReport.Period).FirstOrDefaultAsync();
        }

        public async Task<ReportType> GetByName(string name)
        {
            var context = new ApplicationDbContext();
            return await context.ReportTypes.Where(x => x.Name == name).Include(x => x.Periods)
                .ThenInclude(x => x.Period).FirstOrDefaultAsync();
        }

        public async Task<int> Save(ReportType model)
        {
            var context = new ApplicationDbContext();

            if (model.IsNew)
            {
                model.Id = Guid.NewGuid().ToString();
                await context.ReportTypes.AddAsync(model);
            }
            else
                context.ReportTypes.Update(model);

            return await context.SaveChangesAsync();
        }

        public async Task<int> Remove(string id)
        {
            var context = new ApplicationDbContext();
            var item = await GetById(id);
            context.ReportTypes.Remove(item);
            return await context.SaveChangesAsync();
        }
    }
}