using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Facade;
using forest_report_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace forest_report_api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("intervals")]
    [ApiController]
    public class IntervalController : Controller
    {
        private readonly UserIntervalFacade _userIntervalFacade;
        private readonly AccountServiceFacade _accountServiceFacade;
        
        public IntervalController(UserIntervalFacade userIntervalFacade,
            AccountServiceFacade accountServiceFacade)
        {
            _userIntervalFacade = userIntervalFacade;
            _accountServiceFacade = accountServiceFacade;
        }

        [HttpGet("{year},{reportTypeId}")]
        public async Task<object> GetIntervals(int year, string reportTypeId)
        {
            return await _userIntervalFacade.GetIntervals(year, reportTypeId);
        }

        [HttpPost]
        public async Task<object> Interval(IntervalModel model)
        {
            return await _userIntervalFacade.SaveInterval(model);
        }

        [HttpPost("copy")]
        public async Task<object> CopyIntervals(CopyIntervalModel copyIntervalModel)
        {
            return await _userIntervalFacade.CopyIntervals(copyIntervalModel);
        }
    }
}