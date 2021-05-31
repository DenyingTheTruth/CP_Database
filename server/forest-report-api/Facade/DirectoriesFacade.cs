using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Extensions;
using forest_report_api.Models;
using forest_report_api.Service;

namespace forest_report_api.Facade
{
    public class DirectoriesFacade
    {
        private readonly DirectoriesService _directoriesService;

        public DirectoriesFacade(DirectoriesService directoriesService)
        {
            _directoriesService = directoriesService;
        }

        #region TypeActivity

        public async Task<List<TypeActivity>> GetTypeActivities() =>
            await _directoriesService.GetTypeActivities();

        public async Task<ResponseResult> SaveTypeActivities(TypeActivity model)
        {
            return await _directoriesService.SaveTypeActivities(model);
        }

        public async Task<TypeActivity> GetTypeActivity(string id) =>
            await _directoriesService.GetTypeActivity(id);

        public async Task<ResponseResult> RemoveTypeActivity(string id)
        {
            return await _directoriesService.RemoveTypeActivity(id);
        }

        #endregion

        #region Report Type

        public async Task<object> GetReportTypes() =>
            (await _directoriesService.GetReportTypes()).Select(x => x.GetFrontObject());

        public async Task<ResponseResult> SaveReportType(ReportType model)
        {
            return await _directoriesService.SaveReportType(model);
        }

        public async Task<object> GetReportType(string id) =>
            (await _directoriesService.GetReportType(id)).GetFrontObject();

        public async Task<object> GetReportTypeByName(string name) =>
            (await _directoriesService.GetReportTypeByName(name))?.GetFrontObject() ?? new object();

        public async Task<ResponseResult> RemoveReportType(string id)
        {
            return await _directoriesService.RemoveReportType(id);
        }

        #endregion

        #region Period

        public async Task<object> GetPeriods() =>
            (await _directoriesService.GetPeriods()).Select(x => x.GetFrontObject());

        public async Task<ResponseResult> SavePeriod(Period model)
        {
            return await _directoriesService.SavePeriod(model);
        }

        public async Task<object> GetPeriod(string id) =>
            (await _directoriesService.GetPeriod(id)).GetFrontObject();

        public async Task<ResponseResult> RemovePeriod(string id)
        {
            return await _directoriesService.RemovePeriod(id);
        }

        #endregion

        #region UserCheckinInterval

        public async Task<List<UserCheckinInterval>> GetUserCheckinIntervals() =>
            await _directoriesService.GetUserCheckinIntervals();

        public async Task<ResponseResult> SaveUserCheckinInterval(UserCheckinInterval model)
        {
            return await _directoriesService.SaveUserCheckinInterval(model);
        }

        public async Task<UserCheckinInterval> GetUserCheckinInterval(string id) =>
            await _directoriesService.GetUserCheckinInterval(id);

        public async Task<ResponseResult> RemoveUserCheckinInterval(string id)
        {
            return await _directoriesService.RemoveUserCheckinInterval(id);
        }

        #endregion
    }
}