using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;
using forest_report_api.Models;
using forest_report_api.Repositories;

namespace forest_report_api.Service
{
    public class UserIntervalService : IUserIntervalService
    {
        private readonly IUserCheckinIntervalRepository _checkinIntervalRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public UserIntervalService(IUserCheckinIntervalRepository checkinIntervalRepository,
            IMapper mapper, IReportRepository reportRepository)
        {
            _checkinIntervalRepository = checkinIntervalRepository;
            _mapper = mapper;
            _reportRepository = reportRepository;
        }

        public async Task<UserCheckinInterval> GetUserIntervalById(string id)
        {
            return await _checkinIntervalRepository.GetById(id);
        }

        public async Task<IEnumerable<UserIntervalModel>> GetUserIntervals(string organizationId, int? year = null)
        {
            var resultItems = new List<UserIntervalModel>();
            var intervals = await _checkinIntervalRepository.GetByOrganization(organizationId, year);
            for (var i = 0; i < intervals.Count; i++)
            {
                if (i == 0)
                {
                    intervals[i].Date = intervals[i].EndDate.HasValue
                        ? new DateTime(intervals[i].EndDate!.Value.Year, 1, 1)
                        : (DateTime?) null;
                    continue;
                }

                intervals[i].Date = intervals[i - 1].EndDate.HasValue && intervals[i].EndDate.HasValue
                    ? intervals[i - 1].EndDate
                    : null;
            }

            foreach (var interval in intervals)
            {
                var report = await _reportRepository.GetReportByInterval(interval.Id);
                resultItems.Add(new UserIntervalModel
                {
                    Id = interval.Id,
                    Date = interval.Date,
                    Name = interval.Name,
                    Organization = interval.Organization,
                    Period = interval.Period,
                    Year = interval.Year,
                    EndDate = interval.EndDate,
                    OrganizationId = interval.OrganizationId,
                    PeriodId = interval.PeriodId,
                    CanBeCreated = report != null && report.StatusReport == StatusReport.New || report == null
                });
            }

            return resultItems;
        }

        public async Task<List<UserIntervalModel>> GetIntervals(int year, string reportTypeId)
        {
            var intervals = await _checkinIntervalRepository.GetByYearAndReportType(year, reportTypeId);
            return intervals.Select(x => _mapper.Map<UserIntervalModel>(x)).ToList();
        }

        public async Task<ResponseResult> SaveInterval(IntervalModel model)
        {
            try
            {
                foreach (var periodModel in model.Periods)
                {
                    var entity = new UserCheckinInterval
                    {
                        Id = periodModel.UserCheckinIntervalId,
                        OrganizationId = model.OrganizationId,
                        EndDate = periodModel.Date,
                        PeriodId = periodModel.Id,
                        Year = model.Year
                    };
                    await _checkinIntervalRepository.Save(entity);
                }

                return new ResponseResult(true, "Saved");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<ResponseResult> SaveInterval(UserCheckinInterval model)
        {
            try
            {
                await _checkinIntervalRepository.Save(model);
                return new ResponseResult(true, "Saved");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<List<UserIntervalModel>> GetFreeIntervals(string organizationId, string reportTypeId,
            int year)
        {
            var resultModel = new List<UserIntervalModel>();
            var intervals = await _checkinIntervalRepository
                .GetFree(organizationId, reportTypeId, year);
            foreach (var interval in intervals)
            {
                var newModel = _mapper.Map<UserIntervalModel>(interval);
                if (newModel != null)
                {
                    var report = await _reportRepository.GetReportByInterval(newModel.Id);
                    newModel.CanBeCreated = report == null || report.StatusReport == StatusReport.New;
                }
                resultModel.Add(newModel);
            }

            return resultModel;
        }

        public async Task<IEnumerable<FullUserIntervalModel>> GetFullInfo(int year, string reportTypeId)
        {
            var resultModel = new List<FullUserIntervalModel>();
            var intervals = await _checkinIntervalRepository.GetByYearAndReportType(year, reportTypeId);
            foreach (var interval in intervals)
            {
                var newModel = _mapper.Map<FullUserIntervalModel>(interval);
                if (newModel != null)
                    newModel.Report = await _reportRepository.GetReportByInterval(newModel.Id);
                resultModel.Add(newModel);
            }

            return resultModel.Where(x =>
                x.Report != null && x.Report.StatusReport != StatusReport.New || x.Report == null);
        }
    }
}