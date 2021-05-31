using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Extensions;
using forest_report_api.Models;
using forest_report_api.Repositories;
using forest_report_api.Service;

namespace forest_report_api.Facade
{
    public class UserIntervalFacade
    {
        private readonly IUserIntervalService _userIntervalService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly DirectoriesService _directoriesService;

        public UserIntervalFacade(IUserIntervalService userIntervalService,
            IOrganizationRepository organizationRepository,
            DirectoriesService directoriesService)
        {
            _userIntervalService = userIntervalService;
            _organizationRepository = organizationRepository;
            _directoriesService = directoriesService;
        }

        public async Task<object> GetUserIntervals(string organizationId)
        {
            return (await _userIntervalService.GetUserIntervals(organizationId)).Select(x => x.GetFrontObject());
        }

        public async Task<object> GetIntervalById(string id)
        {
            return await _userIntervalService.GetUserIntervalById(id);
        }
        
        public async Task<object> GetIntervals(int year, string reportTypeId)
        {
            var intervals = await _userIntervalService.GetIntervals(year, reportTypeId);
            var organizations = await _organizationRepository.GetAll();
            var reportType = await _directoriesService.GetReportType(reportTypeId);
            return new ListUserIntervalModel(intervals, organizations, year,
                reportType.Periods.Select(x => x.Period).OrderBy(x => x.Name));
        }

        public async Task<ResponseResult> SaveInterval(IntervalModel intervalModel)
        {
            return await _userIntervalService.SaveInterval(intervalModel);
        }

        public async Task<ResponseResult> CopyIntervals(CopyIntervalModel copyIntervalModel)
        {
            try
            {
                var modelByOrg = (await _userIntervalService
                        .GetUserIntervals(copyIntervalModel.FromOrganizationId)).Where(x =>
                        x.Period.ReportTypes.Any(
                            reportType => reportType.ReportTypeId == copyIntervalModel.ReportTypeId) &&
                        x.Year == copyIntervalModel.Year)
                    .ToList();

                foreach (var toOrganizationId in copyIntervalModel.ToOrganizations)
                {
                    var intervals = await _userIntervalService
                        .GetUserIntervals(toOrganizationId, copyIntervalModel.Year);

                    foreach (var model in modelByOrg)
                    {
                        if (intervals.Any(x => x.PeriodId == model.PeriodId))
                        {
                            var saveInterval = intervals.FirstOrDefault(x => x.PeriodId == model.PeriodId);
                            if (saveInterval != null) saveInterval.EndDate = model.EndDate;
                            await _userIntervalService.SaveInterval(saveInterval);
                        }
                        else
                        {
                            var newInterval = new UserCheckinInterval
                            {
                                EndDate = model.EndDate,
                                OrganizationId = toOrganizationId,
                                PeriodId = model.PeriodId,
                                Year = model.Year
                            };
                            await _userIntervalService.SaveInterval(newInterval);
                        }
                    }
                }

                return new ResponseResult(true, "Copied");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> GetFreeIntervals(string organizationId, string reportTypeId, int year)
        {
            var intervals = await _userIntervalService.GetFreeIntervals(organizationId, reportTypeId, year);
            return new ListUserIntervalModel(intervals, organizationId, year).FirstOrDefault();
        }

        public async Task<object> GetFullInfoIntervals(int year, string reportTypeId)
        {
            var intervals = await _userIntervalService.GetFullInfo(year, reportTypeId);
            var organizations = await _organizationRepository.GetAll();
            var reportType = await _directoriesService.GetReportType(reportTypeId);
            return new ListUserIntervalModel(intervals, organizations, year,
                reportType.Periods.Select(x => x.Period).OrderBy(x => x.Name));
        }
    }
}