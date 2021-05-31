using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Models;

namespace forest_report_api.Service
{
    public interface IUserIntervalService
    {
        Task<UserCheckinInterval> GetUserIntervalById(string id);
        Task<IEnumerable<UserIntervalModel>> GetUserIntervals(string organization, int? year = null);
        Task<List<UserIntervalModel>> GetIntervals(int year, string reportTypeId);
        Task<ResponseResult> SaveInterval(IntervalModel model);
        Task<ResponseResult> SaveInterval(UserCheckinInterval model);
        Task<List<UserIntervalModel>> GetFreeIntervals(string organizationId, string reportTypeId, int year);
        Task<IEnumerable<FullUserIntervalModel>> GetFullInfo(int year, string reportTypeId);
    }
}