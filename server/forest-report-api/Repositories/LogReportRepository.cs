using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Context;
using forest_report_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class LogReportRepository : ILogReportRepository
    {
        public async Task<List<LogReport>> GetAll()
        {
            var context = new ApplicationDbContext();

            return await (from logReport in context.LogReports
                select new LogReport
                {
                    Id = logReport.Id,
                    Date = logReport.Date,
                    ReportId = logReport.ReportId,
                    StatusReport = logReport.StatusReport,
                    AdminStatusReport = logReport.AdminStatusReport
                }).Include(x => x.ApplicationUser).ToListAsync();
        }

        public async Task<LogReport> GetById(string id)
        {
            var context = new ApplicationDbContext();

            return await (from logReport in context.LogReports
                where logReport.Id == id
                select new LogReport
                {
                    Id = logReport.Id,
                    Date = logReport.Date,
                    ReportId = logReport.ReportId,
                    StatusReport = logReport.StatusReport,
                    AdminStatusReport = logReport.AdminStatusReport
                }).Include(x => x.ApplicationUser).FirstOrDefaultAsync();
        }

        public async Task<int> Save(LogReport model)
        {
            var context = new ApplicationDbContext();

            if (model.IsNew)
            {
                model.Id = Guid.NewGuid().ToString();
                await context.LogReports.AddAsync(model);
            }
            else
                context.LogReports.Update(model);

            return await context.SaveChangesAsync();
        }

        public async Task<int> Remove(string id)
        {
            var context = new ApplicationDbContext();
            var item = await GetById(id);
            context.LogReports.Remove(item);
            return await context.SaveChangesAsync();
        }

        public async Task<List<LogReport>> GetAll(string userId)
        {
            var context = new ApplicationDbContext();

            return await context.LogReports
                .Include(x => x.Report)
                .ThenInclude(x => x.ReportType)
                .Include(x => x.Report.UserCheckinInterval)
                .ThenInclude(x => x.Period)
                .Include(x => x.ApplicationUser)
                .Where(x => x.ApplicationUserId == userId)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }
    }
}