using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Context;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;
using forest_report_api.Models;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class UserCheckinIntervalRepository : IUserCheckinIntervalRepository
    {
        public async Task<List<UserCheckinInterval>> GetAll()
        {
            var context = new ApplicationDbContext();

            return await (from checkin in context.UserCheckinIntervals
                join period in context.Periods on checkin.PeriodId equals period.Id
                select new UserCheckinInterval
                {
                    Id = checkin.Id,
                    Date = checkin.Date,
                    Period = new Period
                    {
                        Id = period.Id,
                        Name = period.Name,
                        // ReportType = new ReportType
                        // {
                        //     Id = reportType.Id,
                        //     Name = reportType.Name
                        // }
                    },
                    Year = checkin.Year,
                    PeriodId = checkin.PeriodId,
                    EndDate = checkin.EndDate
                }).ToListAsync();
        }

        public async Task<UserCheckinInterval> GetById(string id)
        {
            var context = new ApplicationDbContext();

            return await (from checkin in context.UserCheckinIntervals
                join period in context.Periods on checkin.PeriodId equals period.Id
                // join reportType in context.ReportTypes on period.ReportTypeId equals reportType.Id
                join organization in context.Organizations on checkin.OrganizationId equals organization.Id
                where checkin.Id == id
                select new UserCheckinInterval
                {
                    Id = checkin.Id,
                    Date = checkin.Date,
                    Period = new Period
                    {
                        Id = period.Id,
                        Name = period.Name,
                        // ReportType = new ReportType
                        // {
                        //     Id = reportType.Id,
                        //     Name = reportType.Name
                        // }
                    },
                    Year = checkin.Year,
                    PeriodId = checkin.PeriodId,
                    Organization = organization
                }).FirstOrDefaultAsync();
        }

        public async Task<int> Save(UserCheckinInterval model)
        {
            var context = new ApplicationDbContext();

            if (model.IsNew)
            {
                model.Id = Guid.NewGuid().ToString();
                await context.UserCheckinIntervals.AddAsync(model);
            }
            else
                context.UserCheckinIntervals.Update(model);

            return await context.SaveChangesAsync();
        }

        public async Task<int> Remove(string id)
        {
            var context = new ApplicationDbContext();
            var item = await GetById(id);
            context.UserCheckinIntervals.Remove(item);
            return await context.SaveChangesAsync();
        }

        public async Task<List<UserCheckinInterval>> GetByOrganization(string organizationId, int? year = null)
        {
            var context = new ApplicationDbContext();

            return await (from checkin in context.UserCheckinIntervals
                join period in context.Periods on checkin.PeriodId equals period.Id
                join organization in context.Organizations on checkin.OrganizationId equals organization.Id
                where checkin.OrganizationId == organizationId && (year != null && checkin.Year == year || year == null)
                select new UserCheckinInterval
                {
                    Id = checkin.Id,
                    Date = checkin.Date,
                    Period = new Period
                    {
                        Id = period.Id,
                        Name = period.Name,
                        ReportTypes = (from periodReportType in context.PeriodReportTypes
                            where periodReportType.PeriodId == period.Id
                            select new PeriodReportType
                            {
                                PeriodId = periodReportType.PeriodId,
                                ReportTypeId = periodReportType.ReportTypeId,
                                Period = (from per in context.Periods.Where(x => x.Id == periodReportType.PeriodId)
                                    select per).FirstOrDefault(),
                                ReportType =
                                    (from type in context.ReportTypes.Where(x => x.Id == periodReportType.ReportTypeId)
                                        select type).FirstOrDefault()
                            })
                    },
                    Year = checkin.Year,
                    PeriodId = checkin.PeriodId,
                    EndDate = checkin.EndDate,
                    OrganizationId = checkin.OrganizationId,
                    Organization = organization
                }).ToListAsync();
        }

        public async Task<List<UserCheckinInterval>> GetByYearAndReportType(int year, string reportTypeId)
        {
            var context = new ApplicationDbContext();

            return await (from checkin in context.UserCheckinIntervals
                join period in context.Periods on checkin.PeriodId equals period.Id
                join organization in context.Organizations on checkin.OrganizationId equals organization.Id
                join periodReportType in context.PeriodReportTypes on period.Id equals periodReportType.PeriodId
                where checkin.Year == year && periodReportType.ReportTypeId == reportTypeId
                select new UserCheckinInterval
                {
                    Id = checkin.Id,
                    Date = checkin.Date,
                    Period = new Period
                    {
                        Id = period.Id,
                        Name = period.Name,
                        ReportTypes = (from periodReportType in context.PeriodReportTypes
                            where periodReportType.PeriodId == period.Id
                            select new PeriodReportType
                            {
                                PeriodId = periodReportType.PeriodId,
                                ReportTypeId = periodReportType.ReportTypeId,
                                Period = (from per in context.Periods.Where(x => x.Id == periodReportType.PeriodId)
                                    select per).FirstOrDefault(),
                                ReportType =
                                    (from type in context.ReportTypes.Where(x => x.Id == periodReportType.ReportTypeId)
                                        select type).FirstOrDefault()
                            })
                    },
                    Year = checkin.Year,
                    PeriodId = checkin.PeriodId,
                    EndDate = checkin.EndDate,
                    OrganizationId = checkin.OrganizationId,
                    Organization = organization
                }).ToListAsync();
        }

        public async Task<List<UserCheckinInterval>> GetFree(string organizationId, string reportTypeId, int year)
        {
            var context = new ApplicationDbContext();

            return await (from checkin in context.UserCheckinIntervals
                join period in context.Periods on checkin.PeriodId equals period.Id
                join organization in context.Organizations on checkin.OrganizationId equals organization.Id
                join periodReportType in context.PeriodReportTypes on period.Id equals periodReportType.PeriodId
                join report in context.Reports on checkin.Id equals report.UserCheckinIntervalId into repUs
                from report in repUs.DefaultIfEmpty()
                where checkin.Year == year 
                      && periodReportType.ReportTypeId == reportTypeId 
                      && checkin.OrganizationId == organizationId
                      && checkin.EndDate != null
                select new UserCheckinInterval
                {
                    Id = checkin.Id,
                    Date = checkin.Date,
                    Period = new Period
                    {
                        Id = period.Id,
                        Name = period.Name,
                        ReportTypes = (from periodReportType in context.PeriodReportTypes
                            where periodReportType.PeriodId == period.Id
                            select new PeriodReportType
                            {
                                PeriodId = periodReportType.PeriodId,
                                ReportTypeId = periodReportType.ReportTypeId,
                                Period = (from per in context.Periods.Where(x => x.Id == periodReportType.PeriodId)
                                    select per).FirstOrDefault(),
                                ReportType =
                                    (from type in context.ReportTypes.Where(x => x.Id == periodReportType.ReportTypeId)
                                        select type).FirstOrDefault()
                            })
                    },
                    Year = checkin.Year,
                    PeriodId = checkin.PeriodId,
                    EndDate = checkin.EndDate,
                    OrganizationId = checkin.OrganizationId,
                    Organization = organization
                }).ToListAsync();
        }

        // public async Task<List<UserCheckinInterval>> GetForCreationByRole(string role)
        // {
        //     var context = new ApplicationDbContext();
        //
        //     return await (from checkin in context.UserCheckinIntervals
        //         join period in context.Periods on checkin.PeriodId equals period.Id
        //         where checkin.Role == role && checkin.ReportId == null
        //         select new UserCheckinInterval
        //         {
        //             Id = checkin.Id,
        //             Date = checkin.Date,
        //             Period = new Period
        //             {
        //                 Id = period.Id,
        //                 Name = period.Name,
        //                 // ReportType = new ReportType
        //                 // {
        //                 //     Id = reportType.Id,
        //                 //     Name = reportType.Name
        //                 // }
        //             },
        //             Year = checkin.Year,
        //             PeriodId = checkin.PeriodId,
        //             Role = checkin.Role,
        //             EndDate = checkin.EndDate
        //         }).ToListAsync();
        // }

        // public async Task<List<UserCheckinInterval>> GetByPeriodYear(string userId, string periodId, int year)
        // {
        //     var context = new ApplicationDbContext();
        //
        //     return await (from checkin in context.UserCheckinIntervals
        //         join period in context.Periods on checkin.PeriodId equals period.Id
        //         join reportType in context.ReportTypes on period.ReportTypeId equals reportType.Id
        //         where checkin.UserId == userId && checkin.PeriodId == periodId && checkin.Year == year
        //         select new UserCheckinInterval
        //         {
        //             Id = checkin.Id,
        //             Date = checkin.Date,
        //             Period = new Period
        //             {
        //                 Id = period.Id,
        //                 Name = period.Name,
        //                 ReportTypeId = period.ReportTypeId,
        //                 ReportType = new ReportType
        //                 {
        //                     Id = reportType.Id,
        //                     Name = reportType.Name
        //                 }
        //             },
        //             Year = checkin.Year,
        //             PeriodId = checkin.PeriodId,
        //             UserId = checkin.UserId,
        //             EndDate = checkin.EndDate
        //         }).ToListAsync();
        // }
    }
}