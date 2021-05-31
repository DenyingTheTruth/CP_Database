using System;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Facade;
using forest_report_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace forest_report_api.Controllers
{
    [Authorize]
    [Route("reports")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly ReportServiceFacade _reportServiceFacade;
        private readonly AccountServiceFacade _accountServiceFacade;
        private readonly UserIntervalFacade _userIntervalFacade;

        public ReportController(ReportServiceFacade reportServiceFacade,
            AccountServiceFacade accountServiceFacade, UserIntervalFacade userIntervalFacade)
        {
            _reportServiceFacade = reportServiceFacade;
            _accountServiceFacade = accountServiceFacade;
            _userIntervalFacade = userIntervalFacade;
        }

        [HttpGet("{id}")]
        public async Task<object> GetReport(string id)
        {
            return await _reportServiceFacade.GetReport(id);
        }

        [HttpGet]
        public async Task<object> GetReports()
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return User.IsInRole("MainUser") || User.IsInRole("Admin")
                ? await _reportServiceFacade.GetReports()
                : await _reportServiceFacade.GetReports(userId);
        }
        
        [HttpGet("/reports-revision")]
        public async Task<object> GetReportsRevision()
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.GetReportsRevision(userId);
        }
        
        [HttpGet("tab/{reportTypeName}")]
        public async Task<object> GetTab(string reportTypeName, int? number = null, string intervalId = null)
        {
            var organizationId = _accountServiceFacade.GetOrganizationIdByName(User.Identity?.Name);
            return number.HasValue
                ? await _reportServiceFacade.GetTab(organizationId, reportTypeName, number, intervalId)
                : _reportServiceFacade.GetTabs(reportTypeName);
        }

        [HttpPost("save")]
        public async Task<object> SaveReport(ReportModel model)
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.SaveReport(model, userId);
        }

        [HttpPost("validate")]
        public async Task<object> ValidateReport(ReportModel model)
        {
            return await _reportServiceFacade.ValidateReport(model);
        }

        [HttpPost("send")]
        public async Task<object> SendReport(ReportModel reportId)
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.SendReport(reportId, userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("send-to-correction")]
        public async Task<object> SendReportToCorrection(ReportModel model)
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.SendForCorrection(model, userId);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("send-reports-to-correction")]
        public async Task<object> SendArrayReportToCorrection([FromBody]string[] reportIds)
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.SendForCorrection(reportIds, userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("return-reports-to-correction")]
        public async Task<object> ReturnReportsToCorrection([FromBody] string[] reportIds)
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.ReturnReportToCorrection(reportIds,userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sent")]
        public async Task<object> SentReports()
        {
            return await _reportServiceFacade.GetSentReports();
        }

        [HttpGet("free/{year},{reportTypeId}")]
        public async Task<object> GetFreeIntervals(int year, string reportTypeId)
        {
            var organizationId = _accountServiceFacade.GetOrganizationIdByName(User.Identity?.Name);
            return await _userIntervalFacade.GetFreeIntervals(organizationId, reportTypeId, year);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("accept")]
        public async Task<object> AcceptReport(ReportModel model)
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.AcceptReport(model, userId);
        }

        [HttpPost("export")]
        public async Task<object> ExportFile(ReportModel model)
        {
            return await _reportServiceFacade.ExportFile(model);
        }
        
        [HttpGet("export-by-id/{reportId}")]
        public async Task<object> ExportFileById(string reportId)
        {
            return await _reportServiceFacade.ExportFile(reportId);
        }

        [HttpPost("import")]
        public async Task<object> ImportFile(byte[] file)
        {
            var organization = await _accountServiceFacade.GetOrganizationByUserName(User.Identity?.Name);
            return _reportServiceFacade.ImportFile(file, organization);
        }

        [HttpPost("import-by-quarter")]
        public async Task<object> ImportFile(ImportDataModel model)
        {
            var organization = await _accountServiceFacade.GetOrganizationByUserName(User.Identity?.Name);
            var interval = (UserCheckinInterval) await _userIntervalFacade.GetIntervalById(model.Id);
            return _reportServiceFacade.ImportFileByPeriod(model.File, organization,
                int.Parse(interval?.Period?.Name[^1].ToString() ?? "0"), interval?.Year);
        }

        [HttpGet("file/{reportId}")]
        public async Task<object> GetFile(string reportId)
        {
            return await _reportServiceFacade.GetFile(reportId);
        }

        [HttpGet("/report-by-interval/{userCheckinIntervalId}")]
        public async Task<object> ReportByInterval(string userCheckinIntervalId)
        {
            return await _reportServiceFacade.GetReportByInterval(userCheckinIntervalId);
        }

        [HttpGet("logs")]
        public async Task<object> ReportLogs()
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.GetLogReports(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("accepted/{year},{reportTypeId}")]
        public async Task<object> GetAcceptedReports(int year, string reportTypeId)
        {
            return await _userIntervalFacade.GetFullInfoIntervals(year, reportTypeId);
        }

        [HttpGet("count-corrections")]
        public async Task<int> GetCountCorrections()
        {
            var userId = _accountServiceFacade.GetUserIdByName(User.Identity?.Name);
            return await _reportServiceFacade.GetCountReportsRevision(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("statistics")]
        public async Task<object> GetReportStatistics(ReportStatisticsFilterModel filter)
        {
            return await _reportServiceFacade.GetReportStatisticsByFilter(filter);
		}

        [Authorize(Roles = "Admin")]
        [HttpGet("count-unread-reports")]
        public async Task<int> GetCountUnreadReports()
        {
            return await _reportServiceFacade.GetCountUnreadReports();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("read/{reportId}")]
        public async Task<object> ReadReport(string reportId)
        {
            return await _reportServiceFacade.ReadReport(reportId);
        }

        [HttpGet("{year},{periodId},{reportTypeId},{organizationId}")]
        public async Task<object> GetReportByPeriod(int year, string periodId, string reportTypeId, string organizationId)
        {
            return await _reportServiceFacade.GetReportByPeriod(year, periodId, reportTypeId, organizationId);
        }
    }
}