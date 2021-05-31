using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Facade;
using forest_report_api.Models;
using forest_report_api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace forest_report_api.Controllers
{
    [Authorize]
    [ApiController]
    public class DirectoriesController
    {
        private readonly DirectoriesFacade _directoriesFacade;

        public DirectoriesController(DirectoriesFacade directoriesFacade)
        {
            _directoriesFacade = directoriesFacade;
        }

        #region TypeActivity

        [HttpGet("type-activities")]
        public async Task<List<TypeActivity>> TypeActivities()
        {
            return await _directoriesFacade.GetTypeActivities();
        }

        [HttpGet("type-activities/{id}")]
        public async Task<TypeActivity> GetTypeActivity(string id)
        {
            return await _directoriesFacade.GetTypeActivity(id);
        }

        #endregion

        #region ReportType

        [HttpGet("report-types")]
        public async Task<object> ReportTypes()
        {
            return await _directoriesFacade.GetReportTypes();
        }

        [HttpGet("report-types/{id}")]
        public async Task<object> GetReportType(string id)
        {
            return await _directoriesFacade.GetReportType(id);
        }
        
        [HttpGet("report-type/{name}")]
        public async Task<object> GetReportTypeByName(string name)
        {
            return await _directoriesFacade.GetReportTypeByName(name);
        }

        #endregion

        #region Period

        [HttpGet("periods")]
        public async Task<object> Periods()
        {
            return await _directoriesFacade.GetPeriods();
        }

        [HttpGet("periods/{id}")]
        public async Task<object> GetPeriod(string id)
        {
            return await _directoriesFacade.GetPeriod(id);
        }

        #endregion
    }
}