using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using forest_report_api.Context;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;
using forest_report_api.Extensions;
using forest_report_api.Models;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class ReportRepository : IReportRepository
    {
        public async Task<List<Report>> GetAll()
        {
            var context = new ApplicationDbContext();

            return await context.Reports
                .Include(x => x.ReportType)
                .Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization)
                .Include(x => x.UserCheckinInterval.Period)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();
        }

        public async Task<Report> GetById(string id)
        {
            var context = new ApplicationDbContext();

            return await context.Reports
                .Include(x => x.ReportType)
                .Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization)
                .Include(x => x.UserCheckinInterval.Period)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Save(Report model)
        {
            var context = new ApplicationDbContext();

            if (model.IsNew)
            {
                model.Id = Guid.NewGuid().ToString();
                await context.Reports.AddAsync(model);
            }
            else
            {
                var oldModel = context.Reports.AsNoTracking().First(x => x.Id == model.Id);
                var fileNeedRemove = false;
                if (oldModel.AttachmentFileId != model.AttachmentFile?.Id)
                {
                    if (oldModel.AttachmentFileId != null)
                        fileNeedRemove = true;
                    if (model.AttachmentFile?.Id != null)
                        await context.Files.AddAsync(model.AttachmentFile);
                    model.AttachmentFileId = model.AttachmentFile?.Id;
                    await context.SaveChangesAsync();
                }

                context.Reports.Update(model);
                await context.SaveChangesAsync();
                if (fileNeedRemove)
                    context.Files.Remove(context.Files.First(x => x.Id == oldModel.AttachmentFileId));
            }

            return await context.SaveChangesAsync();
        }

        public async Task<int> Remove(string id)
        {
            var context = new ApplicationDbContext();
            var item = await GetById(id);
            context.Reports.Remove(item);
            return await context.SaveChangesAsync();
        }

        public async Task<Report> GetReport(string id)
        {
            var context = new ApplicationDbContext();

            return await context.Reports
                .Include(x => x.ReportType)
                .Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization)
                .Include(x => x.UserCheckinInterval.Period)
                .Include(x => x.AttachmentFile)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Report> GetReportByInterval(string userCheckinIntervalId)
        {
            var context = new ApplicationDbContext();

            return await context.Reports
                .Include(x => x.ReportType)
                .Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization)
                .Include(x => x.UserCheckinInterval.Period)
                .Include(x => x.AttachmentFile)
                .Select(x => new Report
                {
                    Id = x.Id,
                    Date = x.Date,
                    Description = x.Description,
                    User = x.User,
                    AttachmentFile = x.AttachmentFileId != null
                        ? new AttachmentFile
                        {
                            Id = x.AttachmentFile.Id,
                            Name = x.AttachmentFile.Name,
                            Type = x.AttachmentFile.Type
                        }
                        : null,
                    ReplyDate = x.ReplyDate,
                    ReportType = x.ReportType,
                    StatusReport = x.StatusReport,
                    UserId = x.UserId,
                    AdminStatusReport = x.AdminStatusReport,
                    AttachmentFileId = x.AttachmentFileId,
                    FormCollectionId = x.FormCollectionId,
                    ReportTypeId = x.ReportTypeId,
                    UserCheckinInterval = x.UserCheckinInterval,
                    UserCheckinIntervalId = x.UserCheckinIntervalId,
                    Note = x.Note,
                    CreationDate = x.CreationDate,
                    IsRead = x.IsRead,
                })
                .FirstOrDefaultAsync(x => x.UserCheckinIntervalId == userCheckinIntervalId);
        }

        public async Task<IEnumerable<Report>> GetReportsByInterval(string userCheckinIntervalId)
        {
            var context = new ApplicationDbContext();

            return await context.Reports
                .Include(x => x.ReportType)
                .Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization)
                .Include(x => x.UserCheckinInterval.Period)
                .Include(x => x.AttachmentFile)
                .Select(x => new Report
                {
                    Id = x.Id,
                    Date = x.Date,
                    Description = x.Description,
                    User = x.User,
                    AttachmentFile = x.AttachmentFileId != null
                        ? new AttachmentFile
                        {
                            Id = x.AttachmentFile.Id,
                            Name = x.AttachmentFile.Name,
                            Type = x.AttachmentFile.Type
                        }
                        : null,
                    ReplyDate = x.ReplyDate,
                    ReportType = x.ReportType,
                    StatusReport = x.StatusReport,
                    UserId = x.UserId,
                    AdminStatusReport = x.AdminStatusReport,
                    AttachmentFileId = x.AttachmentFileId,
                    FormCollectionId = x.FormCollectionId,
                    ReportTypeId = x.ReportTypeId,
                    UserCheckinInterval = x.UserCheckinInterval,
                    UserCheckinIntervalId = x.UserCheckinIntervalId,
                    Note = x.Note,
                    CreationDate = x.CreationDate,
                    IsRead = x.IsRead,
                })
                .Where(x => x.UserCheckinIntervalId == userCheckinIntervalId).ToListAsync();
        }

        public async Task<int> GetReportCount(Expression<Func<Report, bool>> predicate)
        {
            var context = new ApplicationDbContext();
            
            return await context.Reports.CountAsync(predicate);
        }

        public async Task SetReportRead(string id, bool read)
        {
            var context = new ApplicationDbContext();

            var report = context.Reports.FirstOrDefault(x => x.Id.Equals(id));
            report.IsRead = read;

            await context.SaveChangesAsync();
        }

        public async Task<List<Report>> GetSentReports()
        {
            var context = new ApplicationDbContext();

            return (await context.Reports.Where(x =>
                    x.AdminStatusReport != null && x.AdminStatusReport != AdminStatusReport.Accepted)
                .Include(x => x.ReportType)
                .Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization)
                .Include(x => x.UserCheckinInterval.Period)
                .Include(x => x.AttachmentFile)
                .Select(x => new Report
                {
                    Id = x.Id,
                    Date = x.Date,
                    Description = x.Description,
                    User = x.User,
                    AttachmentFile = x.AttachmentFileId != null
                        ? new AttachmentFile
                        {
                            Id = x.AttachmentFile.Id,
                            Name = x.AttachmentFile.Name,
                            Type = x.AttachmentFile.Type
                        }
                        : null,
                    ReplyDate = x.ReplyDate,
                    ReportType = x.ReportType,
                    StatusReport = x.StatusReport,
                    UserId = x.UserId,
                    AdminStatusReport = x.AdminStatusReport,
                    AttachmentFileId = x.AttachmentFileId,
                    FormCollectionId = x.FormCollectionId,
                    ReportTypeId = x.ReportTypeId,
                    UserCheckinInterval = x.UserCheckinInterval,
                    UserCheckinIntervalId = x.UserCheckinIntervalId,
                    Note = x.Note,
                    CreationDate = x.CreationDate,
                    IsRead = x.IsRead,
                }).OrderByDescending(x => x.CreationDate)
                .ToListAsync());
        }

        public async Task<List<Report>> GetAll(string userId)
        {
            var context = new ApplicationDbContext();

            return await context.Reports
                .Where(x =>
                    userId != null && x.UserId == userId ||
                    userId == null)
                .Include(x => x.ReportType)
                .Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization)
                .Include(x => x.UserCheckinInterval.Period)
                .OrderByDescending(x => x.CreationDate)
                .Include(x => x.AttachmentFile)
                .Select(x => new Report
                {
                    Id = x.Id,
                    Date = x.Date,
                    Description = x.Description,
                    User = x.User,
                    AttachmentFile = x.AttachmentFileId != null
                        ? new AttachmentFile
                        {
                            Id = x.AttachmentFile.Id,
                            Name = x.AttachmentFile.Name,
                            Type = x.AttachmentFile.Type
                        }
                        : null,
                    ReplyDate = x.ReplyDate,
                    ReportType = x.ReportType,
                    StatusReport = x.StatusReport,
                    UserId = x.UserId,
                    AdminStatusReport = x.AdminStatusReport,
                    AttachmentFileId = x.AttachmentFileId,
                    FormCollectionId = x.FormCollectionId,
                    ReportTypeId = x.ReportTypeId,
                    UserCheckinInterval = x.UserCheckinInterval,
                    UserCheckinIntervalId = x.UserCheckinIntervalId,
                    Note = x.Note,
                    CreationDate = x.CreationDate,
                    IsRead = x.IsRead,
                })
                .ToListAsync();
        }

        public async Task<List<Report>> GetAllByStatus(StatusReport userStatusReport, string userId = null)
        {
            var context = new ApplicationDbContext();

            return await context.Reports
                .Where(x =>
                    ((userId != null && x.UserId == userId) ||
                     userId == null) && x.StatusReport == userStatusReport)
                .Include(x => x.ReportType)
                .Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization)
                .Include(x => x.UserCheckinInterval.Period)
                .OrderByDescending(x => x.CreationDate)
                .Include(x => x.AttachmentFile)
                .Select(x => new Report
                {
                    Id = x.Id,
                    Date = x.Date,
                    Description = x.Description,
                    User = x.User,
                    AttachmentFile = x.AttachmentFileId != null
                        ? new AttachmentFile
                        {
                            Id = x.AttachmentFile.Id,
                            Name = x.AttachmentFile.Name,
                            Type = x.AttachmentFile.Type
                        }
                        : null,
                    ReplyDate = x.ReplyDate,
                    ReportType = x.ReportType,
                    StatusReport = x.StatusReport,
                    UserId = x.UserId,
                    AdminStatusReport = x.AdminStatusReport,
                    AttachmentFileId = x.AttachmentFileId,
                    FormCollectionId = x.FormCollectionId,
                    ReportTypeId = x.ReportTypeId,
                    UserCheckinInterval = x.UserCheckinInterval,
                    UserCheckinIntervalId = x.UserCheckinIntervalId,
                    Note = x.Note,
                    CreationDate = x.CreationDate,
                    IsRead = x.IsRead,
                })
                .ToListAsync();
        }

        public async Task<object> GetReportStatistics(ReportStatisticsFilterModel filter)
        {
            var context = new ApplicationDbContext();

            return new
            {
                validReports = await context.Reports.Include(x => x.UserCheckinInterval)
                    .Include(x => x.UserCheckinInterval)
                    .ThenInclude(x => x.Organization).ThenInclude(x => x.TypeActivity)
                    .Where(x => x.UserCheckinInterval.Year == filter.Year &&
                                x.UserCheckinInterval.PeriodId == filter.PeriodId &&
                                x.ReportTypeId == filter.ReportTypeId &&
                                x.AdminStatusReport == AdminStatusReport.Accepted)
                    .ToArrayAsync()
            };
        }
        
        public async Task<object> GetReportStatisticsByFilter(ReportStatisticsFilterModel filter)
        {
            var context = new ApplicationDbContext();

            var reports = context.Reports.Include(x => x.UserCheckinInterval).Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization).ThenInclude(x => x.TypeActivity)
                .Where(x => x.UserCheckinInterval.Year == filter.Year &&
                            x.UserCheckinInterval.PeriodId == filter.PeriodId &&
                            x.ReportTypeId == filter.ReportTypeId && x.AdminStatusReport == AdminStatusReport.Accepted);

            var validReports = filter.Filter.FirstOrDefault() switch
            {
                ReportStatisticsFilterEnum.Full => await reports.ToArrayAsync(),
                ReportStatisticsFilterEnum.PercentageOfStateOwnership => await reports.ToArrayAsync(),
                ReportStatisticsFilterEnum.WithoutOrganizationGroup => await reports.Where(x =>
                        !x.UserCheckinInterval.Organization.Name.Contains("концерн",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.Industrial => await reports
                    .Where(x => x.UserCheckinInterval.Organization.TypeActivity.IsIndustrial)
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.Woodworking => await reports.Where(x =>
                        x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                            "деревообрабатывающие предприятия",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.PPIEnterprises => await reports.Where(x =>
                        x.UserCheckinInterval.Organization.TypeActivity.Name.Contains("предприятия цбп",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.Furniture => await reports.Where(x =>
                        x.UserCheckinInterval.Organization.TypeActivity.Name.Contains("мебельные предприятия",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.UnIndustrial => await reports
                    .Where(x => !x.UserCheckinInterval.Organization.TypeActivity.IsIndustrial)
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.LoggingOrganizations => await reports.Where(x =>
                        x.UserCheckinInterval.Organization.TypeActivity.Name.Contains("лесозаготовительные организации",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.TradeEnterprises => await reports.Where(x =>
                        x.UserCheckinInterval.Organization.TypeActivity.Name.Contains("торговые предприятия",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.Others => await reports.Where(x =>
                        x.UserCheckinInterval.Organization.TypeActivity.Name.Contains("прочие",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.PercentageMoreThenHalf => await reports
                    .Where(x => x.UserCheckinInterval.Organization.IsState)
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.PercentageLessThenHalf => await reports
                    .Where(x => !x.UserCheckinInterval.Organization.IsState)
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.IsDOHolding => await reports
                    .Where(x => x.UserCheckinInterval.Organization.IsHolding)
                    .ToArrayAsync(),
                _ => await reports.ToArrayAsync()
            };

            var validOrganization = validReports.Select(x => x.UserCheckinInterval.Organization).ToArray();
            var validOrganizationIds = validOrganization.Select(x => x.Id).ToArray();
            var othersOrganization =
                context.Organizations.Where(x => !validOrganizationIds.Contains(x.Id));

            var invalidOrganization = filter.Filter.FirstOrDefault() switch
            {
                ReportStatisticsFilterEnum.Full => await othersOrganization.ToArrayAsync(),
                ReportStatisticsFilterEnum.PercentageOfStateOwnership => await othersOrganization.ToArrayAsync(),
                ReportStatisticsFilterEnum.WithoutOrganizationGroup => await othersOrganization
                    .Where(x => !x.Name.Contains("концерн", StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.Industrial => await othersOrganization
                    .Where(x => x.TypeActivity.IsIndustrial)
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.Woodworking => await othersOrganization.Where(x =>
                        x.TypeActivity.Name.Contains("деревообрабатывающие предприятия",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.PPIEnterprises => await othersOrganization.Where(x =>
                        x.TypeActivity.Name.Contains("предприятия цбп", StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.Furniture => await othersOrganization.Where(x =>
                        x.TypeActivity.Name.Contains("мебельные предприятия", StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.UnIndustrial => await othersOrganization
                    .Where(x => !x.TypeActivity.IsIndustrial)
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.LoggingOrganizations => await othersOrganization.Where(x =>
                        x.TypeActivity.Name.Contains("лесозаготовительные организации",
                            StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.TradeEnterprises => await othersOrganization.Where(x =>
                        x.TypeActivity.Name.Contains("торговые предприятия", StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.Others => await othersOrganization
                    .Where(x => x.TypeActivity.Name.Contains("прочие", StringComparison.OrdinalIgnoreCase))
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.PercentageMoreThenHalf => await othersOrganization.Where(x => x.IsState)
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.PercentageLessThenHalf => await othersOrganization.Where(x => !x.IsState)
                    .ToArrayAsync(),
                ReportStatisticsFilterEnum.IsDOHolding => await othersOrganization.Where(x => x.IsHolding)
                    .ToArrayAsync(),
                _ => await othersOrganization.ToArrayAsync()
            };

            return new
            {
                validReports,
                validOrganization,
                invalidOrganization
            };
        }

        public async Task<object> GetReportStatisticsByMultiFilter(ReportStatisticsFilterModel filter)
        {
            var context = new ApplicationDbContext();

            var reports = context.Reports.Include(x => x.UserCheckinInterval).Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization).ThenInclude(x => x.TypeActivity)
                .Where(x => x.UserCheckinInterval.Year == filter.Year &&
                            x.UserCheckinInterval.PeriodId == filter.PeriodId &&
                            x.ReportTypeId == filter.ReportTypeId && x.AdminStatusReport == AdminStatusReport.Accepted);
            
            foreach (var filterEnum in filter.Filter)
            {
                switch (filterEnum)
                {
                    case ReportStatisticsFilterEnum.Industrial:
                         reports = reports.Where(x=>x.UserCheckinInterval.Organization.TypeActivity.IsIndustrial);
                         continue;
                    case ReportStatisticsFilterEnum.Woodworking:
                        reports = reports.Where(x => x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                            "деревообрабатывающие предприятия", StringComparison.OrdinalIgnoreCase));
                        continue;
                    case ReportStatisticsFilterEnum.PPIEnterprises:
                        reports = reports.Where(x => x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                            "предприятия цбп",
                            StringComparison.OrdinalIgnoreCase));
                        continue;
                    case ReportStatisticsFilterEnum.Furniture:
                        reports = reports.Where(x => x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                            "мебельные предприятия",
                            StringComparison.OrdinalIgnoreCase));
                        continue;
                    case ReportStatisticsFilterEnum.UnIndustrial:
                        reports = reports.Where(x => !x.UserCheckinInterval.Organization.TypeActivity.IsIndustrial);
                        continue;
                    case ReportStatisticsFilterEnum.LoggingOrganizations:
                        reports = reports.Where(x => x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                            "лесозаготовительные организации",
                            StringComparison.OrdinalIgnoreCase));
                        continue;
                    case ReportStatisticsFilterEnum.TradeEnterprises:
                        reports = reports.Where(x => x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                            "торговые предприятия",
                            StringComparison.OrdinalIgnoreCase));
                        continue;
                    case ReportStatisticsFilterEnum.Others:
                        reports = reports.Where(x => x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                            "прочие",
                            StringComparison.OrdinalIgnoreCase));
                        continue;
                    default:
                        continue;
                }
            }

            var validReports = await reports.ToArrayAsync();
            
            return new
            {
                validReports,
            };
        }

        public async Task<string[]> GetReportIdByMultiFilter(ReportStatisticsFilterModel filter)
        {
            var context = new ApplicationDbContext();
        
            var reports = context.Reports.Include(x => x.UserCheckinInterval).Include(x => x.UserCheckinInterval)
                .ThenInclude(x => x.Organization).ThenInclude(x => x.TypeActivity)
                .Where(x => x.UserCheckinInterval.Year == filter.Year &&
                            x.UserCheckinInterval.PeriodId == filter.PeriodId &&
                            x.ReportTypeId == filter.ReportTypeId && x.AdminStatusReport == AdminStatusReport.Accepted);

            var result = new List<Report>();
            
            foreach (var filterEnum in filter.Filter)
            {
                switch (filterEnum)
                {
                    case ReportStatisticsFilterEnum.Industrial:
                        result.AddRange(
                            reports.Where(x => x.UserCheckinInterval.Organization.TypeActivity.IsIndustrial));
                        continue;
                    case ReportStatisticsFilterEnum.Woodworking:
                        result.AddRange(reports.Where(x =>
                            x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "деревообрабатывающие предприятия", StringComparison.OrdinalIgnoreCase)));
                        continue;
                    case ReportStatisticsFilterEnum.PPIEnterprises:
                        result.AddRange(reports.Where(x =>
                            x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "предприятия цбп",
                                StringComparison.OrdinalIgnoreCase)));
                        continue;
                    case ReportStatisticsFilterEnum.Furniture:
                        result.AddRange(reports.Where(x =>
                            x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "мебельные предприятия",
                                StringComparison.OrdinalIgnoreCase)));
                        continue;
                    case ReportStatisticsFilterEnum.UnIndustrial:
                        result.AddRange(
                            reports.Where(x => !x.UserCheckinInterval.Organization.TypeActivity.IsIndustrial));
                        continue;
                    case ReportStatisticsFilterEnum.LoggingOrganizations:
                        result.AddRange(reports = reports.Where(x =>
                            x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "лесозаготовительные организации",
                                StringComparison.OrdinalIgnoreCase)));
                        continue;
                    case ReportStatisticsFilterEnum.TradeEnterprises:
                        result.AddRange(reports.Where(x =>
                            x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "торговые предприятия",
                                StringComparison.OrdinalIgnoreCase)));
                        continue;
                    case ReportStatisticsFilterEnum.Others:
                        result.AddRange(reports.Where(x =>
                            x.UserCheckinInterval.Organization.TypeActivity.Name.Contains(
                                "прочие",
                                StringComparison.OrdinalIgnoreCase)));
                        continue;
                    default:
                        continue;
                }
            }
        
            return result.Select(x => x.Id).ToArray();
        }


        public async Task<Report> GetReportByPeriod(int year, string periodId, string reportTypeId, string organizationId)
        {
            var context = new ApplicationDbContext();

            var report = await context.Reports
                .Include(x => x.UserCheckinInterval).ThenInclude(x => x.Organization).ThenInclude(x => x.TypeActivity)
                .Where(x => x.UserCheckinInterval.Year == year &&
                            x.UserCheckinInterval.PeriodId == periodId &&
                            x.UserCheckinInterval.OrganizationId == organizationId &&
                            x.ReportTypeId == reportTypeId && 
                            x.AdminStatusReport == AdminStatusReport.Accepted).FirstOrDefaultAsync();

            return report;
        }
    }
}