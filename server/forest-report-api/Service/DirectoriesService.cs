using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Extensions;
using forest_report_api.Models;
using forest_report_api.Repositories;

namespace forest_report_api.Service
{
    public class DirectoriesService
    {
        private readonly ITypeActivityRepository _typeActivityRepository;
        private readonly IReportTypeRepository _reportTypeRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IUserCheckinIntervalRepository _userCheckinIntervalRepository;

        public DirectoriesService(ITypeActivityRepository typeActivityRepository,
            IReportTypeRepository reportTypeRepository,
            IPeriodRepository periodRepository,
            IUserCheckinIntervalRepository userCheckinIntervalRepository)
        {
            _typeActivityRepository = typeActivityRepository;
            _reportTypeRepository = reportTypeRepository;
            _periodRepository = periodRepository;
            _userCheckinIntervalRepository = userCheckinIntervalRepository;
        }

        #region TypeActivity

        public async Task<List<TypeActivity>> GetTypeActivities() =>
            await _typeActivityRepository.GetAll();

        public async Task<ResponseResult> SaveTypeActivities(TypeActivity model)
        {
            try
            {
                await _typeActivityRepository.Save(model);
                return new ResponseResult(true, "Saved");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<TypeActivity> GetTypeActivity(string id) =>
            await _typeActivityRepository.GetById(id);

        public async Task<ResponseResult> RemoveTypeActivity(string id)
        {
            try
            {
                await _typeActivityRepository.Remove(id);
                return new ResponseResult(true, "Removed");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        #endregion

        #region Report Type

        public async Task<List<ReportType>> GetReportTypes() =>
            await _reportTypeRepository.GetAll();

        public async Task<ResponseResult> SaveReportType(ReportType model)
        {
            try
            {
                await _reportTypeRepository.Save(model);
                return new ResponseResult(true, "Saved");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<ReportType> GetReportType(string id) =>
            await _reportTypeRepository.GetById(id);

        public async Task<ReportType> GetReportTypeByName(string name) =>
            await _reportTypeRepository.GetByName(name);

        public async Task<ResponseResult> RemoveReportType(string id)
        {
            try
            {
                await _reportTypeRepository.Remove(id);
                return new ResponseResult(true, "Removed");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        #endregion

        #region Period

        public async Task<List<Period>> GetPeriods() =>
            await _periodRepository.GetAll();

        public async Task<ResponseResult> SavePeriod(Period model)
        {
            try
            {
                await _periodRepository.Save(model);
                return new ResponseResult(true, "Saved");
            }
            catch (Exception e)
            {
                return new ResponseResult(true, e.Message);
            }
        }

        public async Task<Period> GetPeriod(string id) =>
            await _periodRepository.GetById(id);

        public async Task<ResponseResult> RemovePeriod(string id)
        {
            try
            {
                await _periodRepository.Remove(id);
                return new ResponseResult(true, "Saved");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        #endregion

        #region UserCheckinInterval

        public async Task<List<UserCheckinInterval>> GetUserCheckinIntervals() =>
            await _userCheckinIntervalRepository.GetAll();

        public async Task<ResponseResult> SaveUserCheckinInterval(UserCheckinInterval model)
        {
            try
            {
                // var exist = await _userCheckinIntervalRepository
                //     .GetByPeriodYear(model.UserId, model.PeriodId, model.Year);
                var exist = new List<UserCheckinInterval>();
                if (exist.Count == 0)
                {
                    await _userCheckinIntervalRepository.Save(model);
                    return new ResponseResult(true, "Saved");
                }
                else
                    return new ResponseResult(false, "User-checkin-interval exists");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<UserCheckinInterval> GetUserCheckinInterval(string id) =>
            await _userCheckinIntervalRepository.GetById(id);

        public async Task<ResponseResult> RemoveUserCheckinInterval(string id)
        {
            try
            {
                await _userCheckinIntervalRepository.Remove(id);
                return new ResponseResult(true, "Removed");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        #endregion
    }
}